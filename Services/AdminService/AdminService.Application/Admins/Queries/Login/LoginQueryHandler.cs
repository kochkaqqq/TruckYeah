using AdminService.Application.Interfaces;
using AdminService.Application.Shared.Dtos;
using AdminService.Application.Shared.Exceptions;
using AdminService.Application.Shared.Utils;
using AdminService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AdminService.Application.Admins.Queries.Login
{
    public class LoginQueryHandler : IRequestHandler<LoginQuery, LoginResponseDto>
    {
        private readonly IDbContext _dbContext;

        public LoginQueryHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<LoginResponseDto> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var admin = await _dbContext.Admins.FirstOrDefaultAsync(a => a.Name == request.Name && a.PasswordHash == PasswordHasher.Hash(request.Password), cancellationToken)
                ?? throw new EntityNotFoundException(nameof(Admin), request.Name);

            var refreshTokenString = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            var refreshToken = new RefreshToken
            {
                Admin = admin,
                TokenHash = TokenHasher.Hash(refreshTokenString),
                ExpireAt = DateTime.UtcNow.AddMonths(1),
                CreateAt = DateTime.UtcNow
            };

            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();

            return new LoginResponseDto
            {
                AccessToken = JwtGenerator.GenerateJwtToken(admin),
                RefreshToken = refreshTokenString
            };
        }
    }
}
