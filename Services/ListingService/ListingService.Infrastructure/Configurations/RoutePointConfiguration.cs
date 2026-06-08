using ListingService.Domain.Models;
using ListingService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ListingService.Infrastructure.Configurations;

public class RoutePointConfiguration : IEntityTypeConfiguration<RoutePoint>
{
    public void Configure(EntityTypeBuilder<RoutePoint> builder)
    {
        builder.HasKey(rp => rp.Id);
        
        builder.Property(rp => rp.Address).IsRequired().HasMaxLength(500);
        builder.Property(rp => rp.Order).IsRequired();
        
        builder.HasOne<CargoEntity>()
            .WithMany(c => c.RoutePoints)
            .HasForeignKey(rp => rp.CargoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}