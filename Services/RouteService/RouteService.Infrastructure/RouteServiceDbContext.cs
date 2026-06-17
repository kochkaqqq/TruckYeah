using Microsoft.EntityFrameworkCore;
using RouteService.Domain.Models;

namespace RouteService.Infrastructure;

public class RouteServiceDbContext : DbContext
{
    public RouteServiceDbContext(DbContextOptions<RouteServiceDbContext> options)
        : base(options)
    {
    }

    public DbSet<RouteCalculation> RouteCalculations { get; set; }
    public DbSet<ResolvedRoutePoint> ResolvedRoutePoints { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RouteServiceDbContext).Assembly);
    }
}
