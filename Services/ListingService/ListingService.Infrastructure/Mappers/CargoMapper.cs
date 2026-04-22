using ListingService.Domain.Models;
using ListingService.Infrastructure.Entities;

namespace ListingService.Infrastructure.Mappers;

public static class CargoMapper
{
    public static Cargo MapToModel(this CargoEntity cargo)
    {
        return new Cargo(cargo.Id, cargo.Title, cargo.Description, 
            cargo.WeightKg, cargo.VolumeM3, cargo.LengthCm, 
            cargo.WidthCm, cargo.HeightCm, cargo.CargoType, 
            cargo.RouteFrom, cargo.RouteTo, cargo.DistanceKm, 
            cargo.LoadDate, cargo.Price, cargo.CreatedAt);
    }

    public static CargoEntity MapToEntity(this Cargo cargo)
    {
        var cargoEntity = new CargoEntity
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
            CreatedAt =   cargo.CreatedAt
        };
        return cargoEntity;
    }
}