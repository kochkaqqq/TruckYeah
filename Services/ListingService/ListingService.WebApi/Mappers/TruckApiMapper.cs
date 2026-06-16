using ListingService.Domain.Models;
using ListingService.WebApi.Contracts.Truck;

namespace ListingService.WebApi.Mappers;

public static class TruckApiMapper
{
    public static Truck MapToModel(this CreateTruckRequest request, Guid userId)
    {
        return new Truck
        {
            UserId = userId,
            Title = request.Title,
            Description = request.Description,
            RouteFrom = request.RouteFrom,
            RouteTo = request.RouteTo,
            CapacityTons = request.CapacityTons,
            VolumeM3 = request.VolumeM3,
            BodyType = request.BodyType,
            LoadingType = request.LoadingType,
            CrewDriversCount = request.CrewDriversCount,
            AdditionalEquipment = request.AdditionalEquipment,
            AvailableFrom = request.AvailableFrom,
            Price = request.Price,
            PaymentType = request.PaymentType,
            AllowBargaining = request.AllowBargaining,
            PrepaymentPercent = request.PrepaymentPercent,
            Visibility = request.Visibility
        };
    }

    public static TruckResponse MapToResponse(this Truck truck)
    {
        return new TruckResponse
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
            AdditionalEquipment = truck.AdditionalEquipment,
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

    public static TruckSearchCriteria MapToCriteria(this TruckSearchQuery query)
    {
        return new TruckSearchCriteria
        {
            RouteFrom = query.RouteFrom,
            RouteTo = query.RouteTo,
            AvailableDate = query.AvailableDate,
            CapacityFrom = query.CapacityFrom,
            CapacityTo = query.CapacityTo,
            VolumeFrom = query.VolumeFrom,
            VolumeTo = query.VolumeTo,
            BodyType = query.BodyType,
            LoadingType = query.LoadingType,
            AdditionalEquipment = query.AdditionalEquipment,
            Page = query.Page,
            PageSize = query.PageSize,
            SortBy = query.SortBy,
            SortDirection = query.SortDirection
        };
    }
}
