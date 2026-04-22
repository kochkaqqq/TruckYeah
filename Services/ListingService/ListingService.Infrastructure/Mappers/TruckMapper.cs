using ListingService.Domain.Models;
using ListingService.Infrastructure.Entities;

namespace ListingService.Infrastructure.Mappers;

public static class TruckMapper
{
    public static Truck MapToModel(this TruckEntity truckEntity)
    {
        return new Truck(truckEntity.Id, truckEntity.Title, truckEntity.Description, 
            truckEntity.BodyType, truckEntity.CapacityKg, truckEntity.VolumeM3, 
            truckEntity.LengthCm, truckEntity.WidthCm,  truckEntity.HeightCm,
            truckEntity.CurrentLocation, truckEntity.RouteFrom, truckEntity.RouteTo,
            truckEntity.RadiusKm, truckEntity.PricePerKm, truckEntity.Currency, truckEntity.CreatedAt);
    }

    public static TruckEntity MapToEntity(this Truck truck)
    {
        var truckEntity = new TruckEntity
        {
            Id = truck.Id,
            Title =  truck.Title,
            Description = truck.Description,
            BodyType =  truck.BodyType,
            CapacityKg =  truck.CapacityKg,
            VolumeM3 =   truck.VolumeM3,
            LengthCm =   truck.LengthCm,
            WidthCm =    truck.WidthCm,
            HeightCm =    truck.HeightCm,
            CurrentLocation = truck.CurrentLocation,
            RouteFrom =  truck.RouteFrom,
            RouteTo =   truck.RouteTo,
            RadiusKm =   truck.RadiusKm,
            PricePerKm = truck.PricePerKm,
            Currency =   truck.Currency,
            CreatedAt =  truck.CreatedAt
        };
        
        return truckEntity;
    }
}