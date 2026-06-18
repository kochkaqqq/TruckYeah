using AdminService.Application.Interfaces;
using AdminService.Application.Shared.Exceptions;
using AdminService.Application.Shared.Utils;
using AdminService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AdminService.Application.Admins.Queries.Refresh
{
    public class RefreshQueryHandler : IRequestHandler<RefreshQuery, string>
    {
        private readonly IDbContext _dbContext;

        public RefreshQueryHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> Handle(RefreshQuery request, CancellationToken cancellationToken)
        {
            var token = await _dbContext.RefreshTokens.Include(rt => rt.Admin).AsNoTracking().FirstOrDefaultAsync(rt => rt.TokenHash == TokenHasher.Hash(request.RefreshToken))
                ?? throw new EntityNotFoundException(nameof(RefreshToken), request.RefreshToken);

            var accessToken = JwtGenerator.GenerateJwtToken(token.Admin);

            return accessToken;
        }
    }
}
