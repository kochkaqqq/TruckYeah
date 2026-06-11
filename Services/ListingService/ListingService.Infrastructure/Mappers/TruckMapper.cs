using ListingService.Domain.Models;
using ListingService.Infrastructure.Entities;

namespace ListingService.Infrastructure.Mappers;

public static class TruckMapper
{
    public static Truck MapToModel(this TruckEntity truckEntity)
    {
        return new Truck
        {
            Id = truckEntity.Id,
            UserId = truckEntity.UserId,
            Title = truckEntity.Title,
            Description = truckEntity.Description,
            RouteFrom = truckEntity.RouteFrom,
            RouteTo = truckEntity.RouteTo,
            CapacityTons = truckEntity.CapacityTons,
            VolumeM3 = truckEntity.VolumeM3,
            BodyType = truckEntity.BodyType,
            LoadingType = truckEntity.LoadingType,
            CrewDriversCount = truckEntity.CrewDriversCount,
            AvailableFrom = truckEntity.AvailableFrom,
            Price = truckEntity.Price,
            PaymentType = truckEntity.PaymentType,
            AllowBargaining = truckEntity.AllowBargaining,
            PrepaymentPercent = truckEntity.PrepaymentPercent,
            Status = truckEntity.Status,
            Visibility = truckEntity.Visibility,
            CreatedAt = truckEntity.CreatedAt,
            PublishedAt = truckEntity.PublishedAt,
            SourceListingId = truckEntity.SourceListingId
        };
    }

    public static TruckEntity MapToEntity(this Truck truck)
    {
        return new TruckEntity
        {
            Id = truck.Id,
            UserId = truck.UserId,
            Title = truck.Title,
            Description = truck.Description,
            RouteFrom = truck.RouteFrom,
            RouteTo = truck.RouteTo,
            CapacityTons = truck.CapacityTons,
            VolumeM3 = truck.VolumeM3,
            BodyType = truck.BodyType,
            LoadingType = truck.LoadingType,
            CrewDriversCount = truck.CrewDriversCount,
            AvailableFrom = truck.AvailableFrom,
            Price = truck.Price,
            PaymentType = truck.PaymentType,
            AllowBargaining = truck.AllowBargaining,
            PrepaymentPercent = truck.PrepaymentPercent,
            Status = truck.Status,
            Visibility = truck.Visibility,
            CreatedAt = truck.CreatedAt,
            PublishedAt = truck.PublishedAt,
            SourceListingId = truck.SourceListingId
        };
    }
}
