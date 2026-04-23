using Application.Interfaces;
using Application.Shared.Dtos.Requests;
using Application.Shared.Dtos.Responses;
using Application.Shared.Exceptions;
using Application.Shared.HelpFuntions;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IDbContext _dbContext;

        public UserService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<LoginDtoResponse> LoginUserAsync(LoginDtoRequest logDto)
        {
            if (logDto.Email == null && logDto.Phone == null)
            {
                throw new ArgumentException("Either email or phone must be provided.");
            }

            User? user = null;

            if (logDto.Email != null)
            {
                user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Value == logDto.Email);
            }
            else if (logDto.Phone != null)
            {
                user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Phone.FullNumberWithPlus == logDto.Phone); //TODO process input phone number
            }

            if (user == null)
            {
                throw new EntityNotFoundException(nameof(User), logDto.Email ?? logDto.Phone);
            }

            if (!PasswordHasher.Verify(logDto.Password, user.PasswordHash))
                throw new UnauthorizedException("Invalid credentials.");

            var refreshTokenString = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            var refreshToken = new RefreshToken
            {
                User = user,
                TokenHash = TokenHasher.Hash(refreshTokenString),
                ExpireAt = DateTime.UtcNow.AddMonths(1),
                CreateAt = DateTime.UtcNow,
                DeviceId = logDto.DeviceId,
                UserAgent = logDto.UserAgent,
                IpAddress = logDto.IpAddress
            };

            await _dbContext.RefreshTokens.AddAsync(refreshToken);

            await _dbContext.SaveChangesAsync();

            return new LoginDtoResponse
            {
                JwtToken = GenerateJwtToken(user),
                RefreshToken = refreshTokenString
            };
        }

        public async Task<LoginDtoResponse> RefreshTokenAsync(string rawRefreshToken)
        {
            var hash = TokenHasher.Hash(rawRefreshToken);

            var refreshToken = await _dbContext.RefreshTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.TokenHash == hash);

            if (refreshToken == null)
                throw new EntityNotFoundException(nameof(RefreshToken), rawRefreshToken);

            //TODO Add token leak detection

            if (refreshToken.ExpireAt < DateTime.UtcNow)
                throw new UnauthorizedException("Refresh token expired.");

            refreshToken.RevokedAt = DateTime.UtcNow;

            var newRawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var newRefreshToken = new RefreshToken
            {
                User = refreshToken.User,
                TokenHash = TokenHasher.Hash(newRawToken),
                ExpireAt = DateTime.UtcNow.AddMonths(1),
                CreateAt = DateTime.UtcNow,
                DeviceId = refreshToken.DeviceId,
                UserAgent = refreshToken.UserAgent,
                IpAddress = refreshToken.IpAddress
            };

            await _dbContext.RefreshTokens.AddAsync(newRefreshToken);
            await _dbContext.SaveChangesAsync();

            return new LoginDtoResponse
            {
                JwtToken = GenerateJwtToken(refreshToken.User),
                RefreshToken = newRawToken
            };
        }

        public async Task<LoginDtoResponse> RegistrationUserAsync(RegistrationDtoRequest regDto)
        {
            var emailExists = await _dbContext.Users
                .AnyAsync(u => u.Email.Value == regDto.Email);

            if (emailExists)
                throw new ConflictException(nameof(User), regDto.Email);

            var phoneExists = await _dbContext.Users
                .AnyAsync(u => u.Phone.FullNumberWithPlus == regDto.Phone);

            if (phoneExists)
                throw new ConflictException(nameof(User), regDto.Phone);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = new Email(regDto.Email),
                Phone = new Phone(regDto.Phone),
                FullName = new FullName(regDto.Name, regDto.Surname, regDto.MiddleName),
                PasswordHash = PasswordHasher.Hash(regDto.Password)
            };

            var rawRefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var refreshToken = new RefreshToken
            {
                User = user,
                TokenHash = TokenHasher.Hash(rawRefreshToken),
                ExpireAt = DateTime.UtcNow.AddMonths(1),
                CreateAt = DateTime.UtcNow,
                DeviceId = regDto.DeviceId,
                UserAgent = regDto.UserAgent,
                IpAddress = regDto.IpAddress
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();

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
                new(ClaimTypes.Name, user.FullName.ToString()),
                new(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var jwt = new JwtSecurityToken(
                    issuer: "indentity-service",
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes("secret key")), SecurityAlgorithms.HmacSha256)); //TODO move secret key to configuration

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
