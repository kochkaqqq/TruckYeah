using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Interfaces;
using Application.Options;
using Application.Shared.Dtos.Requests;
using Application.Shared.Dtos.Responses;
using Application.Shared.Exceptions;
using Application.Shared.HelpFuntions;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services;

public sealed class UserService : IUserService
{
    private readonly IDbContext _dbContext;
    private readonly JwtOptions _jwtOptions;

    public UserService(IDbContext dbContext, IOptions<JwtOptions> jwtOptions)
    {
        _dbContext = dbContext;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<LoginDtoResponse> LoginUserAsync(LoginDtoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) && string.IsNullOrWhiteSpace(request.Phone))
        {
            throw new ArgumentException("Either email or phone must be provided.");
        }

        User? user;
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var normalizedEmail = request.Email.Trim();
            user = await UsersWithProfile()
                .FirstOrDefaultAsync(u => u.Email.Value == normalizedEmail);
        }
        else
        {
            var phone = new Phone(request.Phone!);
            user = await UsersWithProfile()
                .FirstOrDefaultAsync(u =>
                    u.Phone.CountryCode == phone.CountryCode &&
                    u.Phone.Number == phone.Number);
        }

        if (user is null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid credentials.");
        }
        EnsureActive(user);

        return await CreateTokenPairAsync(
            user,
            request.DeviceId,
            request.UserAgent,
            request.IpAddress);
    }

    public async Task<LoginDtoResponse> RefreshTokenAsync(string rawRefreshToken)
    {
        if (string.IsNullOrWhiteSpace(rawRefreshToken))
        {
            throw new UnauthorizedException("Refresh token is required.");
        }

        var hash = TokenHasher.Hash(rawRefreshToken);
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        var refreshToken = await _dbContext.RefreshTokens
            .Include(t => t.User)
                .ThenInclude(u => u.Country)
            .Include(t => t.User)
                .ThenInclude(u => u.Company)
            .FirstOrDefaultAsync(t => t.TokenHash == hash);

        if (refreshToken is null ||
            refreshToken.RevokedAt is not null ||
            refreshToken.ExpireAt <= DateTime.UtcNow)
        {
            throw new UnauthorizedException("Refresh token is invalid or expired.");
        }
        EnsureActive(refreshToken.User);

        refreshToken.RevokedAt = DateTime.UtcNow;
        var response = await CreateTokenPairAsync(
            refreshToken.User,
            refreshToken.DeviceId,
            refreshToken.UserAgent,
            refreshToken.IpAddress,
            saveChanges: false);

        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        return response;
    }

    public async Task<LoginDtoResponse> RegistrationUserAsync(RegistrationDtoRequest request)
    {
        var email = new Email(request.Email);
        var phone = new Phone(request.Phone);

        if (await _dbContext.Users.AnyAsync(u => u.Email.Value == email.Value))
        {
            throw new ConflictException(nameof(User), email.Value);
        }

        if (await _dbContext.Users.AnyAsync(u =>
                u.Phone.CountryCode == phone.CountryCode &&
                u.Phone.Number == phone.Number))
        {
            throw new ConflictException(nameof(User), phone.FullNumberWithPlus);
        }

        var country = await _dbContext.Countries
            .FirstOrDefaultAsync(c => c.Id == request.CountryId)
            ?? throw new EntityNotFoundException(nameof(Country), request.CountryId.ToString());

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Phone = phone,
            PasswordHash = PasswordHasher.Hash(request.Password),
            Country = country,
            Postcode = new Postcode(request.Postcode),
            UserType = request.UserType,
            AvatarLink = NormalizeOptional(request.AvatarLink),
            Role = AccountRole.User,
            Status = AccountStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        if (request.UserType == UserType.Business)
        {
            if (string.IsNullOrWhiteSpace(request.VatId))
            {
                throw new ArgumentException("VAT ID is required for business users.");
            }

            user.VatId = new VatId(request.VatId);
            user.Company = await ResolveCompanyAsync(request.CompanyId, request.CompanyName);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Surname))
            {
                throw new ArgumentException("Name and surname are required.");
            }

            user.FullName = new FullName(request.Surname, request.Name, request.MiddleName);
            user.VatId = string.IsNullOrWhiteSpace(request.VatId)
                ? null
                : new VatId(request.VatId);
        }

        await _dbContext.Users.AddAsync(user);
        return await CreateTokenPairAsync(
            user,
            request.DeviceId,
            request.UserAgent,
            request.IpAddress);
    }

    public async Task<UserProfileResponse> GetCurrentUserAsync(Guid id)
    {
        var user = await UsersWithProfile()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id)
            ?? throw new EntityNotFoundException(nameof(User), id.ToString());

        return MapPrivateProfile(user);
    }

    public async Task<UserProfileResponse> UpdateCurrentUserAsync(
        Guid id,
        UpdateUserProfileRequest request)
    {
        var user = await UsersWithProfile()
            .FirstOrDefaultAsync(u => u.Id == id)
            ?? throw new EntityNotFoundException(nameof(User), id.ToString());

        var email = new Email(request.Email);
        var phone = new Phone(request.Phone);

        if (await _dbContext.Users.AnyAsync(u =>
                u.Id != id && u.Email.Value == email.Value))
        {
            throw new ConflictException(nameof(User), email.Value);
        }

        if (await _dbContext.Users.AnyAsync(u =>
                u.Id != id &&
                u.Phone.CountryCode == phone.CountryCode &&
                u.Phone.Number == phone.Number))
        {
            throw new ConflictException(nameof(User), phone.FullNumberWithPlus);
        }

        user.Email = email;
        user.Phone = phone;
        user.Postcode = new Postcode(request.Postcode);
        user.City = string.IsNullOrWhiteSpace(request.City) ? null : new City(request.City);
        user.AvatarLink = NormalizeOptional(request.AvatarLink);

        if (user.UserType == UserType.Business)
        {
            if (string.IsNullOrWhiteSpace(request.CompanyName))
            {
                throw new ArgumentException("Company name is required for business users.");
            }

            user.Company = await FindOrCreateCompanyAsync(request.CompanyName);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Surname))
            {
                throw new ArgumentException("Name and surname are required.");
            }

            user.FullName = new FullName(request.Surname, request.Name, request.MiddleName);
        }

        await _dbContext.SaveChangesAsync();
        return MapPrivateProfile(user);
    }

    public async Task<PublicUserResponse> GetPublicUserAsync(Guid id)
    {
        var user = await UsersWithProfile()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id)
            ?? throw new EntityNotFoundException(nameof(User), id.ToString());

        return new PublicUserResponse
        {
            Id = user.Id,
            DisplayName = GetDisplayName(user),
            UserType = user.UserType,
            City = user.City?.Value,
            Company = user.Company?.Name.Value,
            AvatarLink = user.AvatarLink,
            Rating = user.Rating
        };
    }

    public async Task LogoutAsync(string rawRefreshToken)
    {
        if (string.IsNullOrWhiteSpace(rawRefreshToken))
        {
            return;
        }

        var hash = TokenHasher.Hash(rawRefreshToken);
        var refreshToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == hash);

        if (refreshToken is null || refreshToken.RevokedAt is not null)
        {
            return;
        }

        refreshToken.RevokedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<AdminUserResponse>> GetUsersAsync()
    {
        var users = await UsersWithProfile()
            .AsNoTracking()
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        return users.Select(user => new AdminUserResponse
        {
            Id = user.Id,
            Email = user.Email.Value,
            Phone = user.Phone.FullNumberWithPlus,
            DisplayName = GetDisplayName(user),
            UserType = user.UserType,
            Role = user.Role,
            Status = user.Status,
            City = user.City?.Value,
            Company = user.Company?.Name.Value,
            AvatarLink = user.AvatarLink,
            Rating = user.Rating,
            CreatedAt = user.CreatedAt
        }).ToList();
    }

    public async Task BlockUserAsync(Guid id)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id)
            ?? throw new EntityNotFoundException(nameof(User), id.ToString());

        if (user.Role == AccountRole.Moderator)
        {
            throw new ArgumentException("Moderator accounts cannot be blocked through this endpoint.");
        }

        user.Status = AccountStatus.Blocked;
        var now = DateTime.UtcNow;
        var activeTokens = await _dbContext.RefreshTokens
            .Where(t => t.UserId == id && t.RevokedAt == null)
            .ToListAsync();
        foreach (var token in activeTokens)
        {
            token.RevokedAt = now;
        }
        await _dbContext.SaveChangesAsync();
    }

    public async Task UnblockUserAsync(Guid id)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id)
            ?? throw new EntityNotFoundException(nameof(User), id.ToString());
        user.Status = AccountStatus.Active;
        await _dbContext.SaveChangesAsync();
    }

    private IQueryable<User> UsersWithProfile() =>
        _dbContext.Users
            .Include(u => u.Country)
            .Include(u => u.Company);

    private async Task<Company> ResolveCompanyAsync(Guid? companyId, string? companyName)
    {
        if (companyId.HasValue && companyId.Value != Guid.Empty)
        {
            return await _dbContext.Companies
                .FirstOrDefaultAsync(c => c.Id == companyId.Value)
                ?? throw new EntityNotFoundException(nameof(Company), companyId.Value.ToString());
        }

        if (string.IsNullOrWhiteSpace(companyName))
        {
            throw new ArgumentException("Company name is required for business users.");
        }

        return await FindOrCreateCompanyAsync(companyName);
    }

    private async Task<Company> FindOrCreateCompanyAsync(string companyName)
    {
        var normalizedName = new CompanyName(companyName);
        var existing = await _dbContext.Companies
            .FirstOrDefaultAsync(c => c.Name.Value == normalizedName.Value);

        if (existing is not null)
        {
            return existing;
        }

        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = normalizedName
        };
        await _dbContext.Companies.AddAsync(company);
        return company;
    }

    private async Task<LoginDtoResponse> CreateTokenPairAsync(
        User user,
        string? deviceId,
        string? userAgent,
        string? ipAddress,
        bool saveChanges = true)
    {
        var rawRefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var refreshToken = new RefreshToken
        {
            User = user,
            UserId = user.Id,
            TokenHash = TokenHasher.Hash(rawRefreshToken),
            ExpireAt = DateTime.UtcNow.AddMonths(1),
            CreateAt = DateTime.UtcNow,
            DeviceId = NormalizeClientMetadata(deviceId, "unknown-device", 255),
            UserAgent = NormalizeClientMetadata(userAgent, "unknown", 500),
            IpAddress = NormalizeClientMetadata(ipAddress, "unknown", 45)
        };

        await _dbContext.RefreshTokens.AddAsync(refreshToken);
        if (saveChanges)
        {
            await _dbContext.SaveChangesAsync();
        }

        return new LoginDtoResponse
        {
            JwtToken = GenerateJwtToken(user),
            RefreshToken = rawRefreshToken
        };
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Email.Value),
            new(ClaimTypes.MobilePhone, user.Phone.FullNumberWithPlus),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        if (user.FullName is not null)
        {
            claims.Add(new Claim(ClaimTypes.Name, user.FullName.FullNameString));
        }
        else if (user.Company is not null)
        {
            claims.Add(new Claim(ClaimTypes.Name, user.Company.Name.Value));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var jwt = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Math.Max(_jwtOptions.AccessTokenMinutes, 5)),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    private static UserProfileResponse MapPrivateProfile(User user) =>
        new()
        {
            Id = user.Id,
            Email = user.Email.Value,
            Phone = user.Phone.FullNumberWithPlus,
            Name = user.FullName?.FirstName,
            Surname = user.FullName?.LastName,
            MiddleName = user.FullName?.MiddleName,
            FullName = user.FullName?.FullNameString ?? user.Company?.Name.Value,
            UserType = user.UserType,
            CountryId = user.Country.Id,
            Country = user.Country.Name.Value,
            Postcode = user.Postcode.Value,
            City = user.City?.Value,
            CompanyId = user.Company?.Id,
            Company = user.Company?.Name.Value,
            VatId = user.VatId?.Value,
            AvatarLink = user.AvatarLink,
            Rating = user.Rating,
            IsProfileCompleted = user.UserType == UserType.Business
                ? user.Company is not null
                : user.FullName is not null,
            Role = user.Role,
            Status = user.Status,
            CreatedAt = user.CreatedAt
        };

    private static string GetDisplayName(User user) =>
        user.FullName?.FullNameString
        ?? user.Company?.Name.Value
        ?? $"User {user.Id.ToString()[..8]}";

    private static string NormalizeClientMetadata(
        string? value,
        string fallback,
        int maxLength)
    {
        var normalized = string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        return normalized.Length <= maxLength ? normalized : normalized[..maxLength];
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static void EnsureActive(User user)
    {
        if (user.Status == AccountStatus.Blocked)
        {
            throw new UnauthorizedException("User account is blocked.");
        }
    }
}
