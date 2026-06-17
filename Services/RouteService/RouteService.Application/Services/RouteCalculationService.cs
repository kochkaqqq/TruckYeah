using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using RouteService.Application.Interfaces;
using RouteService.Domain.Enums;
using RouteService.Domain.Models;
using RouteService.Infrastructure.Interfaces;
using RouteService.Infrastructure.Options;

namespace RouteService.Application.Services;

public class RouteCalculationService : IRouteCalculationService
{
    private const int MinRoutePoints = 2;
    private const int MaxRoutePoints = 10;

    private readonly IRouteCalculationsRepository _repository;
    private readonly IRouteProvider _routeProvider;
    private readonly RouteCalculationOptions _options;

    public RouteCalculationService(
        IRouteCalculationsRepository repository,
        IRouteProvider routeProvider,
        IOptions<RouteCalculationOptions> options)
    {
        _repository = repository;
        _routeProvider = routeProvider;
        _options = options.Value;
    }

    public async Task<RouteCalculationResult> CalculateAsync(
        IEnumerable<RoutePointInput> points,
        double? fuelConsumptionLitersPer100Km,
        RouteVehicleOptions? vehicle,
        bool avoidTollRoads,
        bool avoidFerries,
        CancellationToken cancellationToken)
    {
        var normalizedPoints = NormalizeAndValidate(points);
        var fuelRate = NormalizeFuelRate(fuelConsumptionLitersPer100Km);
        var vehicleOptions = NormalizeVehicleOptions(vehicle);
        var resolvedPoints = await ResolvePoints(normalizedPoints, cancellationToken);
        var requestHash = BuildRequestHash(resolvedPoints, fuelRate, vehicleOptions, avoidTollRoads, avoidFerries);
        var now = DateTime.UtcNow;

        var cached = await _repository.GetActiveByHash(requestHash, now);
        if (cached is not null)
        {
            return MapCachedResult(cached);
        }

        var providerResult = await _routeProvider.CalculateRouteAsync(
            resolvedPoints,
            vehicleOptions,
            avoidTollRoads,
            avoidFerries,
            cancellationToken);
        var fuelConsumption = Math.Round(providerResult.DistanceKm * fuelRate / 100d, 1, MidpointRounding.AwayFromZero);

        var calculationId = Guid.NewGuid();
        foreach (var point in resolvedPoints)
        {
            point.RouteCalculationId = calculationId;
        }

        var calculation = new RouteCalculation
        {
            Id = calculationId,
            Provider = RouteProvider.OpenRouteService,
            RequestHash = requestHash,
            DistanceKm = providerResult.DistanceKm,
            DurationMinutes = providerResult.DurationMinutes,
            FuelConsumptionLiters = fuelConsumption,
            TollRoadsStatus = providerResult.TollRoadsStatus,
            Geometry = providerResult.Geometry.GeoJson ?? providerResult.Geometry.Polyline,
            CreatedAt = now,
            ExpiresAt = now.AddHours(Math.Max(_options.CacheTtlHours, 1)),
            ResolvedPoints = resolvedPoints
        };

        await _repository.Create(calculation);

        return new RouteCalculationResult
        {
            DistanceKm = calculation.DistanceKm,
            DurationMinutes = calculation.DurationMinutes,
            FuelConsumptionLiters = calculation.FuelConsumptionLiters,
            TollRoadsStatus = calculation.TollRoadsStatus,
            Provider = calculation.Provider,
            IsEstimated = false,
            FromCache = false,
            ResolvedPoints = calculation.ResolvedPoints.OrderBy(p => p.Order).ToList(),
            Geometry = providerResult.Geometry,
            Warnings = providerResult.Warnings
        };
    }

    private List<RoutePointInput> NormalizeAndValidate(IEnumerable<RoutePointInput> points)
    {
        var normalizedPoints = points
            .OrderBy(p => p.Order)
            .ToList();

        if (normalizedPoints.Count < MinRoutePoints)
        {
            throw new ArgumentException("Route must contain at least two points.");
        }

        if (normalizedPoints.Count > MaxRoutePoints)
        {
            throw new ArgumentException("Route cannot contain more than ten points.");
        }

        foreach (var point in normalizedPoints)
        {
            var hasAddress = !string.IsNullOrWhiteSpace(point.Address);
            var hasCoordinates = point.Lat.HasValue || point.Lon.HasValue;

            if (!hasAddress && !hasCoordinates)
            {
                throw new ArgumentException("Each route point must contain address or coordinates.");
            }

            if (hasCoordinates)
            {
                if (!point.Lat.HasValue || !point.Lon.HasValue)
                {
                    throw new ArgumentException("Both lat and lon are required when coordinates are used.");
                }

                if (point.Lat is < -90 or > 90 || point.Lon is < -180 or > 180)
                {
                    throw new ArgumentException("Route point coordinates are invalid.");
                }
            }
        }

        return normalizedPoints;
    }

    private double NormalizeFuelRate(double? fuelConsumptionLitersPer100Km)
    {
        var fuelRate = fuelConsumptionLitersPer100Km ?? _options.DefaultFuelConsumptionLitersPer100Km;
        if (fuelRate <= 0 || fuelRate > 200)
        {
            throw new ArgumentException("Fuel consumption must be greater than zero and less than or equal to 200 liters per 100 km.");
        }

        return fuelRate;
    }

    private static RouteVehicleOptions? NormalizeVehicleOptions(RouteVehicleOptions? vehicle)
    {
        if (vehicle is null)
        {
            return null;
        }

        ValidatePositiveVehicleValue(vehicle.HeightMeters, "Vehicle height must be greater than zero.");
        ValidatePositiveVehicleValue(vehicle.WidthMeters, "Vehicle width must be greater than zero.");
        ValidatePositiveVehicleValue(vehicle.LengthMeters, "Vehicle length must be greater than zero.");
        ValidatePositiveVehicleValue(vehicle.WeightTons, "Vehicle weight must be greater than zero.");
        ValidatePositiveVehicleValue(vehicle.AxleLoadTons, "Vehicle axle load must be greater than zero.");

        if (!vehicle.HeightMeters.HasValue &&
            !vehicle.WidthMeters.HasValue &&
            !vehicle.LengthMeters.HasValue &&
            !vehicle.WeightTons.HasValue &&
            !vehicle.AxleLoadTons.HasValue &&
            !vehicle.Hazmat)
        {
            return null;
        }

        return new RouteVehicleOptions
        {
            HeightMeters = vehicle.HeightMeters,
            WidthMeters = vehicle.WidthMeters,
            LengthMeters = vehicle.LengthMeters,
            WeightTons = vehicle.WeightTons,
            AxleLoadTons = vehicle.AxleLoadTons,
            Hazmat = vehicle.Hazmat
        };
    }

    private static void ValidatePositiveVehicleValue(double? value, string errorMessage)
    {
        if (value <= 0)
        {
            throw new ArgumentException(errorMessage);
        }
    }

    private async Task<List<ResolvedRoutePoint>> ResolvePoints(IEnumerable<RoutePointInput> points, CancellationToken cancellationToken)
    {
        var resolvedPoints = new List<ResolvedRoutePoint>();

        foreach (var point in points)
        {
            if (point.Lat.HasValue && point.Lon.HasValue)
            {
                resolvedPoints.Add(new ResolvedRoutePoint
                {
                    Id = Guid.NewGuid(),
                    Address = string.IsNullOrWhiteSpace(point.Address)
                        ? $"{point.Lat.Value.ToString(CultureInfo.InvariantCulture)}, {point.Lon.Value.ToString(CultureInfo.InvariantCulture)}"
                        : point.Address.Trim(),
                    Lat = point.Lat.Value,
                    Lon = point.Lon.Value,
                    Order = point.Order
                });
                continue;
            }

            resolvedPoints.Add(await _routeProvider.GeocodeAsync(point.Address!.Trim(), point.Order, cancellationToken));
        }

        return resolvedPoints.OrderBy(p => p.Order).ToList();
    }

    private static string BuildRequestHash(
        IEnumerable<ResolvedRoutePoint> resolvedPoints,
        double fuelRate,
        RouteVehicleOptions? vehicle,
        bool avoidTollRoads,
        bool avoidFerries)
    {
        var vehicleHash = vehicle is null
            ? "none"
            : string.Create(
                CultureInfo.InvariantCulture,
                $"h:{vehicle.HeightMeters?.ToString(CultureInfo.InvariantCulture) ?? "-"};w:{vehicle.WidthMeters?.ToString(CultureInfo.InvariantCulture) ?? "-"};l:{vehicle.LengthMeters?.ToString(CultureInfo.InvariantCulture) ?? "-"};weight:{vehicle.WeightTons?.ToString(CultureInfo.InvariantCulture) ?? "-"};axle:{vehicle.AxleLoadTons?.ToString(CultureInfo.InvariantCulture) ?? "-"};hazmat:{vehicle.Hazmat}");

        var raw = $"provider:{RouteProvider.OpenRouteService}|" +
            string.Join("|", resolvedPoints
            .OrderBy(p => p.Order)
            .Select(p => string.Create(
                CultureInfo.InvariantCulture,
                $"{p.Order}:{Math.Round(p.Lat, 6)}:{Math.Round(p.Lon, 6)}"))) +
            $"|fuel:{Math.Round(fuelRate, 2).ToString(CultureInfo.InvariantCulture)}" +
            $"|avoidToll:{avoidTollRoads}|avoidFerries:{avoidFerries}|vehicle:{vehicleHash}";

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
        return Convert.ToHexString(bytes);
    }

    private static RouteCalculationResult MapCachedResult(RouteCalculation cached)
    {
        return new RouteCalculationResult
        {
            DistanceKm = cached.DistanceKm,
            DurationMinutes = cached.DurationMinutes,
            FuelConsumptionLiters = cached.FuelConsumptionLiters,
            TollRoadsStatus = cached.TollRoadsStatus,
            Provider = cached.Provider,
            IsEstimated = false,
            FromCache = true,
            ResolvedPoints = cached.ResolvedPoints.OrderBy(p => p.Order).ToList(),
            Geometry = MapCachedGeometry(cached.Geometry),
            Warnings = []
        };
    }

    private static RouteGeometry MapCachedGeometry(string? geometry)
    {
        if (string.IsNullOrWhiteSpace(geometry))
        {
            return new RouteGeometry();
        }

        var trimmed = geometry.TrimStart();
        return trimmed.StartsWith('{') || trimmed.StartsWith('[')
            ? new RouteGeometry { GeoJson = geometry }
            : new RouteGeometry { Polyline = geometry };
    }
}
