using ListingService.Domain.Models;
using ListingService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace ListingService.Infrastructure;

public class ListingServiceDbContext : DbContext
{
    public ListingServiceDbContext(DbContextOptions<ListingServiceDbContext> options)
        : base(options)
    {
    }

    public DbSet<CargoEntity> Cargos { get; set; }
    public DbSet<TruckEntity> Trucks { get; set; }
    public DbSet<RoutePoint> RoutePoints { get; set; }
    public DbSet<CargoBidEntity> CargoBids { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ListingServiceDbContext).Assembly);
    }
}
