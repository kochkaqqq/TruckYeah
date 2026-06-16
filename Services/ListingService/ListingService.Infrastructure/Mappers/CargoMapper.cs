using ListingService.Domain.Models;
using ListingService.Infrastructure.Entities;

namespace ListingService.Infrastructure.Mappers;

public static class CargoMapper
{
    public static Cargo MapToModel(this CargoEntity cargoEntity)
    {
        return new Cargo
        {
            Id = cargoEntity.Id,
            UserId = cargoEntity.UserId,
            Title = cargoEntity.Title,
            CargoName = cargoEntity.CargoName,
            RouteFrom = cargoEntity.RouteFrom,
            RouteTo = cargoEntity.RouteTo,
            RoutePoints = cargoEntity.RoutePoints.OrderBy(p => p.Order).ToList(),
            LoadDateTime = cargoEntity.LoadDateTime,
            UnloadDateTime = cargoEntity.UnloadDateTime,
            WeightTons = cargoEntity.WeightTons,
            VolumeM3 = cargoEntity.VolumeM3,
            BodyTypeRequired = cargoEntity.BodyTypeRequired,
            LoadingType = cargoEntity.LoadingType,
            LengthCm = cargoEntity.LengthCm,
            WidthCm = cargoEntity.WidthCm,
            HeightCm = cargoEntity.HeightCm,
            PalletsCount = cargoEntity.PalletsCount,
            PackagingType = cargoEntity.PackagingType,
            RequiresCMR = cargoEntity.RequiresCMR,
            RequiresTIR = cargoEntity.RequiresTIR,
            IsADR = cargoEntity.IsADR,
            RequiresTwoDrivers = cargoEntity.RequiresTwoDrivers,
            PaymentType = cargoEntity.PaymentType,
            AllowBargaining = cargoEntity.AllowBargaining,
            PrepaymentPercent = cargoEntity.PrepaymentPercent,
            StartingPrice = cargoEntity.StartingPrice,
            BiddingEnabled = cargoEntity.BiddingEnabled,
            MinBidStep = cargoEntity.MinBidStep,
            AcceptedBidId = cargoEntity.AcceptedBidId,
            BiddingClosedAt = cargoEntity.BiddingClosedAt,
            Status = cargoEntity.Status,
            Visibility = cargoEntity.Visibility,
            CreatedAt = cargoEntity.CreatedAt,
            PublishedAt = cargoEntity.PublishedAt,
            BoostToTop = cargoEntity.BoostToTop,
            BoostedUntil = cargoEntity.BoostedUntil,
            IsTemplate = cargoEntity.IsTemplate,
            TemplateName = cargoEntity.TemplateName,
            SourceListingId = cargoEntity.SourceListingId,
            Notes = cargoEntity.Notes
        };
    }

    public static CargoEntity MapToEntity(this Cargo cargo)
    {
        return new CargoEntity
        {
            Id = cargo.Id,
            UserId = cargo.UserId,
            Title = cargo.Title,
            CargoName = cargo.CargoName,
            RouteFrom = cargo.RouteFrom,
            RouteTo = cargo.RouteTo,
            RoutePoints = cargo.RoutePoints,
            LoadDateTime = cargo.LoadDateTime,
            UnloadDateTime = cargo.UnloadDateTime,
            WeightTons = cargo.WeightTons,
            VolumeM3 = cargo.VolumeM3,
            BodyTypeRequired = cargo.BodyTypeRequired,
            LoadingType = cargo.LoadingType,
            LengthCm = cargo.LengthCm,
            WidthCm = cargo.WidthCm,
            HeightCm = cargo.HeightCm,
            PalletsCount = cargo.PalletsCount,
            PackagingType = cargo.PackagingType,
            RequiresCMR = cargo.RequiresCMR,
            RequiresTIR = cargo.RequiresTIR,
            IsADR = cargo.IsADR,
            RequiresTwoDrivers = cargo.RequiresTwoDrivers,
            PaymentType = cargo.PaymentType,
            AllowBargaining = cargo.AllowBargaining,
            PrepaymentPercent = cargo.PrepaymentPercent,
            StartingPrice = cargo.StartingPrice,
            BiddingEnabled = cargo.BiddingEnabled,
            MinBidStep = cargo.MinBidStep,
            AcceptedBidId = cargo.AcceptedBidId,
            BiddingClosedAt = cargo.BiddingClosedAt,
            Status = cargo.Status,
            Visibility = cargo.Visibility,
            CreatedAt = cargo.CreatedAt,
            PublishedAt = cargo.PublishedAt,
            BoostToTop = cargo.BoostToTop,
            BoostedUntil = cargo.BoostedUntil,
            IsTemplate = cargo.IsTemplate,
            TemplateName = cargo.TemplateName,
            SourceListingId = cargo.SourceListingId,
            Notes = cargo.Notes
        };
    }
}
