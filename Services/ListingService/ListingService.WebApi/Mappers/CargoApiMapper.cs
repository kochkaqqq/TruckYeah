using ListingService.Domain.Enums;
using ListingService.Domain.Models;
using ListingService.Infrastructure.Entities;
using ListingService.WebApi.Contracts.Cargo;

namespace ListingService.WebApi.Mappers;

public static class CargoApiMapper
{
    public static Cargo MapToModel(this CargoRequest cargoRequest)
    {
        var cargo = new Cargo
        {
            Title = cargoRequest.Title,
            CargoName = cargoRequest.CargoName,
            RouteFrom =  cargoRequest.RouteFrom,
            RouteTo =  cargoRequest.RouteTo,
            RoutePoints =  cargoRequest.RoutePoints,
            LoadDateTime =  cargoRequest.LoadDateTime,
            UnloadDateTime =  cargoRequest.UnloadDateTime,
            WeightTons =  cargoRequest.WeightTons,
            VolumeM3 =  cargoRequest.VolumeM3,
            BodyTypeRequired =   cargoRequest.BodyTypeRequired,
            LengthCm =  cargoRequest.LengthCm,
            WidthCm =  cargoRequest.WidthCm,
            HeightCm =  cargoRequest.HeightCm,
            PalletsCount =  cargoRequest.PalletsCount,
            PackagingType =   cargoRequest.PackagingType,
            RequiresCMR =   cargoRequest.RequiresCMR,
            RequiresTIR =    cargoRequest.RequiresTIR,
            PaymentType =    cargoRequest.PaymentType,
            AllowBargaining =     cargoRequest.AllowBargaining,
            PrepaymentPercent =   cargoRequest.PrepaymentPercent,
            Notes =   cargoRequest.Notes,
        };
        return  cargo;
    }

    public static CargoResponse MapToResponse(this Cargo cargo)
    {
        var cargoResponse = new CargoResponse
        {
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
            LengthCm = cargo.LengthCm,
            WidthCm = cargo.WidthCm,
            HeightCm = cargo.HeightCm,
            PalletsCount = cargo.PalletsCount,
            PackagingType = cargo.PackagingType,
            RequiresCMR = cargo.RequiresCMR,
            RequiresTIR = cargo.RequiresTIR,
            PaymentType = cargo.PaymentType,
            AllowBargaining = cargo.AllowBargaining,
            PrepaymentPercent = cargo.PrepaymentPercent,
            Notes = cargo.Notes,
        };
        return cargoResponse;
    }
}