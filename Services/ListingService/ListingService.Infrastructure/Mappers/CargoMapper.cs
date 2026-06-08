using ListingService.Domain.Models;
using ListingService.Infrastructure.Entities;

namespace ListingService.Infrastructure.Mappers;

public static class CargoMapper
{
    public static Cargo MapToModel(this CargoEntity cargoEntity)
    {
        var cargo = new Cargo
        {
            Id = cargoEntity.Id,
            Title = cargoEntity.Title,
            CargoName =  cargoEntity.CargoName,
            RouteFrom = cargoEntity.RouteFrom,
            RouteTo = cargoEntity.RouteTo,
            RoutePoints =  cargoEntity.RoutePoints,
            LoadDateTime =  cargoEntity.LoadDateTime,
            UnloadDateTime =  cargoEntity.UnloadDateTime,
            WeightTons =  cargoEntity.WeightTons,
            VolumeM3 =  cargoEntity.VolumeM3,
            BodyTypeRequired =   cargoEntity.BodyTypeRequired,
            LoadingType =  cargoEntity.LoadingType,
            LengthCm =  cargoEntity.LengthCm,
            WidthCm =  cargoEntity.WidthCm,
            HeightCm =  cargoEntity.HeightCm,
            PalletsCount = cargoEntity.PalletsCount,
            PackagingType = cargoEntity.PackagingType,
            RequiresCMR = cargoEntity.RequiresCMR,
            RequiresTIR = cargoEntity.RequiresTIR,
            IsADR = cargoEntity.IsADR,
            PaymentType =  cargoEntity.PaymentType,
            AllowBargaining =  cargoEntity.AllowBargaining,
            PrepaymentPercent =   cargoEntity.PrepaymentPercent,
            Status =   cargoEntity.Status,
            CreatedAt =  cargoEntity.CreatedAt,
            PublishedAt =  cargoEntity.PublishedAt,
            BoostToTop =   cargoEntity.BoostToTop,
            BoostedUntil =   cargoEntity.BoostedUntil,
            IsTemplate =   cargoEntity.IsTemplate,
            TemplateName =  cargoEntity.TemplateName,
            SourceListingId =  cargoEntity.SourceListingId,
        };
        
        return cargo;
    }

    public static CargoEntity MapToEntity(this Cargo cargo)
    {
        var cargoEntity = new CargoEntity
        {
            Id = cargo.Id,
            Title = cargo.Title,
            CargoName =  cargo.CargoName,
            RouteFrom = cargo.RouteFrom,
            RouteTo = cargo.RouteTo,
            RoutePoints =  cargo.RoutePoints,
            LoadDateTime =  cargo.LoadDateTime,
            UnloadDateTime =  cargo.UnloadDateTime,
            WeightTons =  cargo.WeightTons,
            VolumeM3 =  cargo.VolumeM3,
            BodyTypeRequired =   cargo.BodyTypeRequired,
            LoadingType =  cargo.LoadingType,
            LengthCm =  cargo.LengthCm,
            WidthCm =  cargo.WidthCm,
            HeightCm =  cargo.HeightCm,
            PalletsCount = cargo.PalletsCount,
            PackagingType = cargo.PackagingType,
            RequiresCMR = cargo.RequiresCMR,
            RequiresTIR = cargo.RequiresTIR,
            IsADR = cargo.IsADR,
            PaymentType =  cargo.PaymentType,
            AllowBargaining =  cargo.AllowBargaining,
            PrepaymentPercent =   cargo.PrepaymentPercent,
            Status =   cargo.Status,
            CreatedAt =  cargo.CreatedAt,
            PublishedAt =  cargo.PublishedAt,
            BoostToTop =   cargo.BoostToTop,
            BoostedUntil =   cargo.BoostedUntil,
            IsTemplate =   cargo.IsTemplate,
            TemplateName =  cargo.TemplateName,
            SourceListingId =  cargo.SourceListingId,
        };
        return cargoEntity;
    }
}