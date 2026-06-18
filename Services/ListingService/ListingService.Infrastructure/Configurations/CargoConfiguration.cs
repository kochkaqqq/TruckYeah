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

        builder.Property(c => c.UserId).IsRequired();
        builder.HasIndex(c => c.UserId);

        builder.Property(c => c.Title).IsRequired().HasMaxLength(200);
        builder.Property(c => c.CargoName).IsRequired().HasMaxLength(200);
        builder.Property(c => c.RouteFrom).IsRequired().HasMaxLength(255);
        builder.Property(c => c.RouteTo).IsRequired().HasMaxLength(255);
        builder.Property(c => c.RouteGeometryGeoJson).HasColumnType("jsonb");
        builder.Property(c => c.LoadDateTime).IsRequired();
        builder.Property(c => c.UnloadDateTime).IsRequired();

        builder.Property(c => c.WeightTons).IsRequired();
        builder.Property(c => c.VolumeM3).IsRequired();
        builder.Property(c => c.UseAutomaticCalculation).HasDefaultValue(false);
        builder.Property(c => c.WeightPerPackageKg);
        builder.Property(c => c.BodyTypeRequired).IsRequired().HasMaxLength(50);
        builder.Property(c => c.LoadingType).IsRequired().HasConversion<string>();

        builder.Property(c => c.PackagingType).HasMaxLength(100);
        builder.Property(c => c.RequiresCMR).HasDefaultValue(false);
        builder.Property(c => c.RequiresTIR).HasDefaultValue(false);
        builder.Property(c => c.IsADR).HasDefaultValue(false);
        builder.Property(c => c.RequiresTwoDrivers).HasDefaultValue(false);
        builder.Property(c => c.Notes).HasMaxLength(1000);

        builder.Property(c => c.PaymentType).IsRequired().HasConversion<string>();
        builder.Property(c => c.AllowBargaining).HasDefaultValue(false);
        builder.Property(c => c.PrepaymentPercent).HasPrecision(5, 2);
        builder.Property(c => c.StartingPrice).HasPrecision(12, 2);
        builder.Property(c => c.BiddingEnabled).HasDefaultValue(false);
        builder.Property(c => c.MinBidStep).HasPrecision(12, 2);
        builder.Property(c => c.AcceptedBidId);
        builder.Property(c => c.BiddingClosedAt);

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(ListingStatus.Draft);

        builder.Property(c => c.Visibility)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(ListingVisibility.Exchange);

        builder.Property(c => c.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(c => c.BoostToTop).HasDefaultValue(false);
        builder.Property(c => c.IsTemplate).HasDefaultValue(false);
        builder.Property(c => c.TemplateName).HasMaxLength(100);

        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.Visibility);
        builder.HasIndex(c => c.LoadDateTime);
        builder.HasIndex(c => c.BiddingEnabled);
        builder.HasIndex(c => c.AcceptedBidId);

        builder.HasMany(c => c.Bids)
            .WithOne(b => b.Cargo)
            .HasForeignKey(b => b.CargoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
