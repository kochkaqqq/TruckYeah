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

        // === ОБЯЗАТЕЛЬНЫЕ ПОЛЯ (из аналитики) ===
        builder.Property(t => t.Title).IsRequired().HasMaxLength(200);
        
        builder.Property(t => t.RouteFrom).IsRequired().HasMaxLength(255);
        builder.Property(t => t.RouteTo).IsRequired().HasMaxLength(255);
        
        builder.Property(t => t.CapacityTons).IsRequired().HasPrecision(10, 3);
        builder.Property(t => t.VolumeM3).IsRequired().HasPrecision(10, 3);
        builder.Property(t => t.BodyType).IsRequired().HasMaxLength(50);
        
        builder.Property(t => t.AvailableFrom).IsRequired();
        builder.Property(t => t.Price).IsRequired().HasPrecision(12, 2); // цена с копейками

        // === ДОПОЛНИТЕЛЬНЫЕ ПОЛЯ ===
        builder.Property(t => t.Description).HasMaxLength(1000);
        
        builder.Property(t => t.LoadingType).HasConversion<string>();

        // === ФИНАНСЫ ===
        builder.Property(t => t.PaymentType).IsRequired().HasConversion<string>();
        builder.Property(t => t.AllowBargaining).HasDefaultValue(false);
        builder.Property(t => t.PrepaymentPercent).HasPrecision(5, 2);

        // === ПУБЛИКАЦИЯ И АРХИВ ===
        builder.Property(t => t.Status).IsRequired().HasConversion<string>().HasDefaultValue(ListingStatus.Draft);
        builder.Property(t => t.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(t => t.PublishedAt);
        
        builder.Property(t => t.SourceListingId);
    }
}