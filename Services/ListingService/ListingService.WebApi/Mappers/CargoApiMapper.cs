using ListingService.Domain.Models;
using ListingService.Infrastructure.Entities;
using ListingService.WebApi.Contracts.Cargo;

namespace ListingService.WebApi.Mappers;

public static class CargoApiMapper
{
    public static Cargo MapToModel(this CargoRequest cargoRequest)
    {
        return new Cargo(new Guid(), cargoRequest.Title, cargoRequest.Description, 
            cargoRequest.WeightKg, cargoRequest.VolumeM3, cargoRequest.LengthCm, 
            cargoRequest.WidthCm, cargoRequest.HeightCm, cargoRequest.CargoType, 
            cargoRequest.RouteFrom, cargoRequest.RouteTo, cargoRequest.DistanceKm, 
            cargoRequest.LoadDate, cargoRequest.Price, DateTime.UtcNow);
    }

    public static CargoResponse MapToResponse(this Cargo cargo)
    {
        var cargoResponse = new CargoResponse()
        {
            Id = cargo.Id,
            Title =  cargo.Title,
            Description = cargo.Description,
            WeightKg = cargo.WeightKg,
            VolumeM3 = cargo.VolumeM3,
            LengthCm = cargo.LengthCm,
            WidthCm =  cargo.WidthCm,
            HeightCm =   cargo.HeightCm,
            CargoType =  cargo.CargoType,
            RouteFrom =  cargo.RouteFrom,
            RouteTo =   cargo.RouteTo,
            DistanceKm =  cargo.DistanceKm,
            LoadDate =   cargo.LoadDate,
            Price =   cargo.Price,
        };
        return cargoResponse;
    }
}