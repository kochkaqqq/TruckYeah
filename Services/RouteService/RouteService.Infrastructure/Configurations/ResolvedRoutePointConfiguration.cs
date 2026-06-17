using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RouteService.Domain.Models;

namespace RouteService.Infrastructure.Configurations;

public class ResolvedRoutePointConfiguration : IEntityTypeConfiguration<ResolvedRoutePoint>
{
    public void Configure(EntityTypeBuilder<ResolvedRoutePoint> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.RouteCalculationId).IsRequired();
        builder.Property(p => p.Address).IsRequired().HasMaxLength(500);
        builder.Property(p => p.Lat).IsRequired();
        builder.Property(p => p.Lon).IsRequired();
        builder.Property(p => p.Order).IsRequired();

        builder.HasIndex(p => new { p.RouteCalculationId, p.Order });
    }
}
