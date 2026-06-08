using ListingService.Domain.Models;
using ListingService.Infrastructure.Entities;

namespace ListingService.Infrastructure.Mappers;

public static class TruckMapper
{
    public static Truck MapToModel(this TruckEntity truckEntity)
    {
        var truck = new Truck
        {
            Id = truckEntity.Id,
            Title = truckEntity.Title,
            Description = truckEntity.Description,
            RouteFrom = truckEntity.RouteFrom,
            RouteTo = truckEntity.RouteTo,
            CapacityTons = truckEntity.CapacityTons,
            VolumeM3 = truckEntity.VolumeM3,
            BodyType = truckEntity.BodyType,
            LoadingType = truckEntity.LoadingType,
            AvailableFrom = truckEntity.AvailableFrom,
            Price = truckEntity.Price,
            PaymentType = truckEntity.PaymentType,
            AllowBargaining = truckEntity.AllowBargaining,
            PrepaymentPercent = truckEntity.PrepaymentPercent,
            Status = truckEntity.Status,
            CreatedAt = truckEntity.CreatedAt,
            PublishedAt = truckEntity.PublishedAt,
            SourceListingId = truckEntity.SourceListingId,
        };
        return truck;
    }

    public static TruckEntity MapToEntity(this Truck truck)
    {
        var truckEntity = new TruckEntity
        {
            Id = truck.Id,
            Title = truck.Title,
            Description = truck.Description,
            RouteFrom = truck.RouteFrom,
            RouteTo = truck.RouteTo,
            CapacityTons = truck.CapacityTons,
            VolumeM3 = truck.VolumeM3,
            BodyType = truck.BodyType,
            LoadingType = truck.LoadingType,
            AvailableFrom = truck.AvailableFrom,
            Price = truck.Price,
            PaymentType = truck.PaymentType,
            AllowBargaining = truck.AllowBargaining,
            PrepaymentPercent = truck.PrepaymentPercent,
            Status = truck.Status,
            CreatedAt = truck.CreatedAt,
            PublishedAt = truck.PublishedAt,
            SourceListingId = truck.SourceListingId,
        };
        
        return truckEntity;
    }
}