using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces
{
    public interface IDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Company> Companies { get; set; }
        DbSet<RefreshToken> RefreshTokens { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
