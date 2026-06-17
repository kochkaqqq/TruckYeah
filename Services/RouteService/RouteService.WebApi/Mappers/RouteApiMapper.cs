using System.Text.Json;
using RouteService.Domain.Models;
using RouteService.WebApi.Contracts;

namespace RouteService.WebApi.Mappers;

public static class RouteApiMapper
{
    public static RoutePointInput MapToModel(this RoutePointInputRequest request)
    {
        return new RoutePointInput
        {
            Address = request.Address,
            Lat = request.Lat,
            Lon = request.Lon,
            Order = request.Order
        };
    }

    public static RouteCalculationResponse MapToResponse(this RouteCalculationResult result)
    {
        return new RouteCalculationResponse
        {
            DistanceKm = result.DistanceKm,
            DurationMinutes = result.DurationMinutes,
            FuelConsumptionLiters = result.FuelConsumptionLiters,
            TollRoadsStatus = result.TollRoadsStatus,
            Provider = result.Provider,
            IsEstimated = result.IsEstimated,
            FromCache = result.FromCache,
            ResolvedPoints = result.ResolvedPoints
                .OrderBy(p => p.Order)
                .Select(p => p.MapToResponse())
                .ToList(),
            Geometry = new RouteGeometryResponse
            {
                Polyline = result.Geometry.Polyline,
                GeoJson = ParseGeoJson(result.Geometry.GeoJson)
            },
            Warnings = result.Warnings
        };
    }

    public static RouteVehicleOptions MapToModel(this RouteVehicleOptionsRequest request)
    {
        return new RouteVehicleOptions
        {
            HeightMeters = request.HeightMeters,
            WidthMeters = request.WidthMeters,
            LengthMeters = request.LengthMeters,
            WeightTons = request.WeightTons,
            AxleLoadTons = request.AxleLoadTons,
            Hazmat = request.Hazmat
        };
    }

    private static ResolvedRoutePointResponse MapToResponse(this ResolvedRoutePoint point)
    {
        return new ResolvedRoutePointResponse
        {
            Address = point.Address,
            Lat = point.Lat,
            Lon = point.Lon,
            Order = point.Order
        };
    }

    private static JsonElement? ParseGeoJson(string? geoJson)
    {
        if (string.IsNullOrWhiteSpace(geoJson))
        {
            return null;
        }

        using var document = JsonDocument.Parse(geoJson);
        return document.RootElement.Clone();
    }
}
