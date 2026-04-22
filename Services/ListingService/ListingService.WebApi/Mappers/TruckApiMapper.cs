using ListingService.Domain.Models;
using ListingService.Infrastructure.Entities;
using ListingService.WebApi.Contracts.Truck;

namespace ListingService.WebApi.Mappers;

public static class TruckApiMapper
{
    public static Truck MapToModel(this TruckRequest truckRequest)
    {
        return new Truck(new Guid(), truckRequest.Title, truckRequest.Description, 
            truckRequest.BodyType, truckRequest.CapacityKg, truckRequest.VolumeM3, 
            truckRequest.LengthCm, truckRequest.WidthCm,  truckRequest.HeightCm,
            truckRequest.CurrentLocation, truckRequest.RouteFrom, truckRequest.RouteTo,
            truckRequest.RadiusKm, truckRequest.PricePerKm, truckRequest.Currency, DateTime.UtcNow);
    }

    public static TruckResponse MapToResponse(this Truck truck)
    {
        var truckResponse = new TruckResponse
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
        };
        
        return truckResponse;
    }
}