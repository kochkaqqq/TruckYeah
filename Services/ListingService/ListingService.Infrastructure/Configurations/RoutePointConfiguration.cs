using ListingService.Domain.Models;
using ListingService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ListingService.Infrastructure.Configurations;

public class RoutePointConfiguration : IEntityTypeConfiguration<RoutePoint>
{
    public void Configure(EntityTypeBuilder<RoutePoint> builder)
    {
        builder.HasKey(point => point.Id);
        builder.Property(point => point.Address).IsRequired().HasMaxLength(500);
        builder.Property(point => point.Lat).IsRequired();
        builder.Property(point => point.Lon).IsRequired();
        builder.Property(point => point.Order).IsRequired();

        builder.HasOne<CargoEntity>()
            .WithMany(cargo => cargo.RoutePoints)
            .HasForeignKey(point => point.CargoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<TruckEntity>()
            .WithMany(truck => truck.RoutePoints)
            .HasForeignKey(point => point.TruckId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(point => new { point.CargoId, point.Order });
        builder.HasIndex(point => new { point.TruckId, point.Order });
    }
}
