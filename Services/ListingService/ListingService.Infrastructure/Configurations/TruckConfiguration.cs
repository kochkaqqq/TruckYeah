using ListingService.Domain.Enums;
using ListingService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ListingService.Infrastructure.Configurations;

public class TruckConfiguration : IEntityTypeConfiguration<TruckEntity>
{
    public void Configure(EntityTypeBuilder<TruckEntity> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.UserId).IsRequired();
        builder.HasIndex(t => t.UserId);

        builder.Property(t => t.Title).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Description).HasMaxLength(1000);
        builder.Property(t => t.RouteFrom).IsRequired().HasMaxLength(255);
        builder.Property(t => t.RouteTo).IsRequired().HasMaxLength(255);
        builder.Property(t => t.CapacityTons).IsRequired();
        builder.Property(t => t.VolumeM3).IsRequired();
        builder.Property(t => t.BodyType).IsRequired().HasMaxLength(50);
        builder.Property(t => t.LoadingType).IsRequired().HasConversion<string>();
        builder.Property(t => t.CrewDriversCount).IsRequired().HasDefaultValue(1);
        builder.Property(t => t.AvailableFrom).IsRequired();
        builder.Property(t => t.Price).IsRequired().HasPrecision(12, 2);

        builder.Property(t => t.PaymentType).IsRequired().HasConversion<string>();
        builder.Property(t => t.AllowBargaining).HasDefaultValue(false);
        builder.Property(t => t.PrepaymentPercent).HasPrecision(5, 2);

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(ListingStatus.Draft);

        builder.Property(t => t.Visibility)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(ListingVisibility.Exchange);

        builder.Property(t => t.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.Visibility);
        builder.HasIndex(t => t.AvailableFrom);
    }
}
