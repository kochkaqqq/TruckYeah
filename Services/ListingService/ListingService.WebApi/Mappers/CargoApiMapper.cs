using ListingService.Domain.Models;
using ListingService.WebApi.Contracts.Cargo;

namespace ListingService.WebApi.Mappers;

public static class CargoApiMapper
{
    public static Cargo MapToModel(this CreateCargoRequest request, Guid userId)
    {
        return new Cargo
        {
            UserId = userId,
            Title = request.Title,
            CargoName = request.CargoName,
            RouteFrom = request.RouteFrom,
            RouteTo = request.RouteTo,
            RoutePoints = request.RoutePoints
                .Select(p => p.MapToModel())
                .OrderBy(p => p.Order)
                .ToList(),
            RouteDistanceKm = request.RouteDistanceKm,
            RouteDurationMinutes = request.RouteDurationMinutes,
            RouteFuelLiters = request.RouteFuelLiters,
            RouteGeometryGeoJson = request.RouteGeometryGeoJson,
            RouteCalculatedAt = request.RouteCalculatedAt,
            LoadDateTime = request.LoadDateTime,
            UnloadDateTime = request.UnloadDateTime,
            WeightTons = request.WeightTons,
            VolumeM3 = request.VolumeM3,
            UseAutomaticCalculation = request.UseAutomaticCalculation,
            WeightPerPackageKg = request.WeightPerPackageKg,
            BodyTypeRequired = request.BodyTypeRequired,
            LoadingType = request.LoadingType,
            LengthCm = request.LengthCm,
            WidthCm = request.WidthCm,
            HeightCm = request.HeightCm,
            PalletsCount = request.PalletsCount,
            PackagingType = request.PackagingType,
            RequiresCMR = request.RequiresCMR,
            RequiresTIR = request.RequiresTIR,
            IsADR = request.IsADR,
            RequiresTwoDrivers = request.RequiresTwoDrivers,
            PaymentType = request.PaymentType,
            AllowBargaining = request.AllowBargaining,
            PrepaymentPercent = request.PrepaymentPercent,
            StartingPrice = request.StartingPrice,
            BiddingEnabled = request.BiddingEnabled,
            MinBidStep = request.MinBidStep,
            Visibility = request.Visibility,
            BoostToTop = request.BoostToTop,
            IsTemplate = request.IsTemplate,
            TemplateName = request.TemplateName,
            SourceListingId = request.SourceListingId,
            Notes = request.Notes
        };
    }

    public static CargoResponse MapToResponse(this Cargo cargo)
    {
        return new CargoResponse
        {
            Id = cargo.Id,
            UserId = cargo.UserId,
            Title = cargo.Title,
            CargoName = cargo.CargoName,
            RouteFrom = cargo.RouteFrom,
            RouteTo = cargo.RouteTo,
            RoutePoints = cargo.RoutePoints
                .OrderBy(p => p.Order)
                .Select(p => p.MapToResponse())
                .ToList(),
            RouteDistanceKm = cargo.RouteDistanceKm,
            RouteDurationMinutes = cargo.RouteDurationMinutes,
            RouteFuelLiters = cargo.RouteFuelLiters,
            RouteGeometryGeoJson = cargo.RouteGeometryGeoJson,
            RouteCalculatedAt = cargo.RouteCalculatedAt,
            LoadDateTime = cargo.LoadDateTime,
            UnloadDateTime = cargo.UnloadDateTime,
            WeightTons = cargo.WeightTons,
            VolumeM3 = cargo.VolumeM3,
            UseAutomaticCalculation = cargo.UseAutomaticCalculation,
            WeightPerPackageKg = cargo.WeightPerPackageKg,
            BodyTypeRequired = cargo.BodyTypeRequired,
            LoadingType = cargo.LoadingType,
            LengthCm = cargo.LengthCm,
            WidthCm = cargo.WidthCm,
            HeightCm = cargo.HeightCm,
            PalletsCount = cargo.PalletsCount,
            PackagingType = cargo.PackagingType,
            RequiresCMR = cargo.RequiresCMR,
            RequiresTIR = cargo.RequiresTIR,
            IsADR = cargo.IsADR,
            RequiresTwoDrivers = cargo.RequiresTwoDrivers,
            PaymentType = cargo.PaymentType,
            AllowBargaining = cargo.AllowBargaining,
            PrepaymentPercent = cargo.PrepaymentPercent,
            StartingPrice = cargo.StartingPrice,
            BiddingEnabled = cargo.BiddingEnabled,
            MinBidStep = cargo.MinBidStep,
            AcceptedBidId = cargo.AcceptedBidId,
            BiddingClosedAt = cargo.BiddingClosedAt,
            Status = cargo.Status,
            Visibility = cargo.Visibility,
            CreatedAt = cargo.CreatedAt,
            PublishedAt = cargo.PublishedAt,
            ModeratedAt = cargo.ModeratedAt,
            ModeratedBy = cargo.ModeratedBy,
            RejectionReason = cargo.RejectionReason,
            BoostToTop = cargo.BoostToTop,
            BoostedUntil = cargo.BoostedUntil,
            IsTemplate = cargo.IsTemplate,
            TemplateName = cargo.TemplateName,
            SourceListingId = cargo.SourceListingId,
            Notes = cargo.Notes
        };
    }

    public static RoutePoint MapToModel(this RoutePointRequest request)
    {
        return new RoutePoint
        {
            Id = Guid.NewGuid(),
            Address = request.Address,
            Lat = request.Lat,
            Lon = request.Lon,
            ScheduledTime = request.ScheduledTime,
            Order = request.Order
        };
    }

    public static RoutePointResponse MapToResponse(this RoutePoint point)
    {
        return new RoutePointResponse
        {
            Id = point.Id,
            Address = point.Address,
            Lat = point.Lat,
            Lon = point.Lon,
            ScheduledTime = point.ScheduledTime,
            Order = point.Order
        };
    }

    public static CargoSearchCriteria MapToCriteria(this CargoSearchQuery query)
    {
        return new CargoSearchCriteria
        {
            RouteFrom = query.RouteFrom,
            RouteTo = query.RouteTo,
            LoadDate = query.LoadDate,
            WeightFrom = query.WeightFrom,
            WeightTo = query.WeightTo,
            VolumeFrom = query.VolumeFrom,
            VolumeTo = query.VolumeTo,
            BodyType = query.BodyType,
            CargoName = query.CargoName,
            LoadingType = query.LoadingType,
            OnlyWithBidding = query.OnlyWithBidding,
            Visibility = query.Visibility,
            Page = query.Page,
            PageSize = query.PageSize,
            SortBy = query.SortBy,
            SortDirection = query.SortDirection
        };
    }

    public static CargoBidResponse MapToResponse(this CargoBid bid)
    {
        return new CargoBidResponse
        {
            Id = bid.Id,
            CargoId = bid.CargoId,
            CarrierUserId = bid.CarrierUserId,
            Price = bid.Price,
            Status = bid.Status,
            CreatedAt = bid.CreatedAt,
            AcceptedAt = bid.AcceptedAt
        };
    }
}
