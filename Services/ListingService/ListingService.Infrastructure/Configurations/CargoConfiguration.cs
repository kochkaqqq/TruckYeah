using ListingService.Domain.Enums;
using ListingService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ListingService.Infrastructure.Configurations;

public class CargoConfiguration : IEntityTypeConfiguration<CargoEntity>
{
    public void Configure(EntityTypeBuilder<CargoEntity> builder)
    {
        builder.HasKey(c => c.Id);

        // === ОБЯЗАТЕЛЬНЫЕ ПОЛЯ (из аналитики) ===
        builder.Property(c => c.Title).IsRequired().HasMaxLength(200);
        builder.Property(c => c.CargoName).IsRequired().HasMaxLength(200);
        
        builder.Property(c => c.RouteFrom).IsRequired().HasMaxLength(255);
        builder.Property(c => c.RouteTo).IsRequired().HasMaxLength(255);
        
        builder.Property(c => c.LoadDateTime).IsRequired();
        // UnloadDateTime — опционально, оставляем nullable
        
        builder.Property(c => c.WeightTons).IsRequired().HasPrecision(10, 3); // до 3 знаков после запятой
        builder.Property(c => c.VolumeM3).IsRequired().HasPrecision(10, 3);
        
        builder.Property(c => c.BodyTypeRequired).IsRequired().HasMaxLength(50);
        builder.Property(c => c.LoadingType).IsRequired(); // enum

        // === ДОПОЛНИТЕЛЬНЫЕ ПОЛЯ ===
        builder.Property(c => c.LengthCm).HasPrecision(8, 2);
        builder.Property(c => c.WidthCm).HasPrecision(8, 2);
        builder.Property(c => c.HeightCm).HasPrecision(8, 2);
        
        builder.Property(c => c.PalletsCount);
        builder.Property(c => c.PackagingType).HasMaxLength(100);
        
        // Документы — булевы флаги (по умолчанию false)
        builder.Property(c => c.RequiresCMR).HasDefaultValue(false);
        builder.Property(c => c.RequiresTIR).HasDefaultValue(false);
        builder.Property(c => c.IsADR).HasDefaultValue(false);
        
        builder.Property(c => c.Notes).HasMaxLength(1000);

        // === ФИНАНСЫ ===
        builder.Property(c => c.PaymentType).IsRequired().HasConversion<string>(); // храним как строку
        builder.Property(c => c.AllowBargaining).HasDefaultValue(false);
        builder.Property(c => c.PrepaymentPercent).HasPrecision(5, 2); // 0.00 - 100.00

        // === ПУБЛИКАЦИЯ И ПРОДВИЖЕНИЕ ===
        builder.Property(c => c.Status).IsRequired().HasConversion<string>().HasDefaultValue(ListingStatus.Draft);
        builder.Property(c => c.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(c => c.PublishedAt);
        
        builder.Property(c => c.BoostToTop).HasDefaultValue(false);
        builder.Property(c => c.BoostedUntil);

        // === ШАБЛОНЫ И АРХИВ ===
        builder.Property(c => c.IsTemplate).HasDefaultValue(false);
        builder.Property(c => c.TemplateName).HasMaxLength(100);
        builder.Property(c => c.SourceListingId);

        // === НАВИГАЦИОННЫЕ СВОЙСТВА ===
        
        // Промежуточные точки маршрута
        builder.HasMany(c => c.RoutePoints)
            .WithOne()
            .HasForeignKey(rp => rp.CargoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}