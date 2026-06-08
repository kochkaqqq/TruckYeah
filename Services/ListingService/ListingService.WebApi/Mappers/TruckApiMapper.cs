using ListingService.Domain.Models;
using ListingService.Infrastructure.Entities;
using ListingService.WebApi.Contracts.Truck;

namespace ListingService.WebApi.Mappers;

public static class TruckApiMapper
{
    public static Truck MapToModel(this TruckRequest truckRequest)
    {
        return new Truck
        {
            Title = truckRequest.Title,
            Description = truckRequest.Description,
            RouteFrom = truckRequest.RouteFrom,
            RouteTo = truckRequest.RouteTo,
            CapacityTons = truckRequest.CapacityTons,
            VolumeM3 = truckRequest.VolumeM3,
            BodyType = truckRequest.BodyType,
            LoadingType = truckRequest.LoadingType,
            AvailableFrom = truckRequest.AvailableFrom,
            Price = truckRequest.Price,
            PaymentType = truckRequest.PaymentType,
            AllowBargaining = truckRequest.AllowBargaining,
            PrepaymentPercent = truckRequest.PrepaymentPercent,
        };
    }

    public static TruckResponse MapToResponse(this Truck truck)
    {
        var truckResponse = new TruckResponse
        {
            Title = truck.Title,
            Description = truck.Description,
            RouteFrom = truck.RouteFrom,
            RouteTo = truck.RouteTo,
            CapacityTons =  truck.CapacityTons,
            VolumeM3 =  truck.VolumeM3,
            BodyType = truck.BodyType,
            LoadingType =  truck.LoadingType,
            AvailableFrom =   truck.AvailableFrom,
            Price =  truck.Price,
            PaymentType = truck.PaymentType,
            AllowBargaining =   truck.AllowBargaining,
            PrepaymentPercent =  truck.PrepaymentPercent,
        };
        return truckResponse;
    }
}