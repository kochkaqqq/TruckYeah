using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RouteService.Domain.Enums;
using RouteService.Domain.Models;

namespace RouteService.Infrastructure.Configurations;

public class RouteCalculationConfiguration : IEntityTypeConfiguration<RouteCalculation>
{
    public void Configure(EntityTypeBuilder<RouteCalculation> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Provider).IsRequired().HasConversion<string>();
        builder.Property(r => r.RequestHash).IsRequired().HasMaxLength(128);
        builder.Property(r => r.DistanceKm).IsRequired();
        builder.Property(r => r.DurationMinutes).IsRequired();
        builder.Property(r => r.FuelConsumptionLiters).IsRequired();
        builder.Property(r => r.TollRoadsStatus).IsRequired().HasConversion<string>().HasDefaultValue(TollRoadsStatus.Unknown);
        builder.Property(r => r.Geometry);
        builder.Property(r => r.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(r => r.ExpiresAt).IsRequired();

        builder.HasIndex(r => r.RequestHash).IsUnique();
        builder.HasIndex(r => r.ExpiresAt);

        builder.HasMany(r => r.ResolvedPoints)
            .WithOne()
            .HasForeignKey(p => p.RouteCalculationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
