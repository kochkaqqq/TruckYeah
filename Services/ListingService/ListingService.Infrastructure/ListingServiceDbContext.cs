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
    public DbSet<TruckEntity>  Trucks { get; set; }
}