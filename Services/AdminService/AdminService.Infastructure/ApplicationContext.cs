using AdminService.Application.Interfaces;
using AdminService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdminService.Infastructure
{
    public class ApplicationContext : DbContext, IDbContext
    {
        public DbSet<Admin> Admins {get; set;}

        public DbSet<RefreshToken> RefreshTokens {get; set;}

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
        }
    }
}
