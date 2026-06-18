using AdminService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AdminService.Application.Interfaces
{
    public interface IDbContext
    {
        DbSet<Admin> Admins { get; }
        DbSet<RefreshToken> RefreshTokens { get; set; }

        DatabaseFacade Database { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
