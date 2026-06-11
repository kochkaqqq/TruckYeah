using ListingService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ListingService.Infrastructure.Configurations;

public class CargoBidConfiguration : IEntityTypeConfiguration<CargoBidEntity>
{
    public void Configure(EntityTypeBuilder<CargoBidEntity> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.CargoId).IsRequired();
        builder.Property(b => b.CarrierUserId).IsRequired();
        builder.Property(b => b.Price).IsRequired().HasPrecision(12, 2);
        builder.Property(b => b.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(b => b.CargoId);
        builder.HasIndex(b => new { b.CargoId, b.CarrierUserId });
    }
}
