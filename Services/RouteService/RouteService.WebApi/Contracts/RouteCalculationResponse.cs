using RouteService.Domain.Enums;

namespace RouteService.WebApi.Contracts;

public class RouteCalculationResponse
{
    public double DistanceKm { get; set; }
    public int DurationMinutes { get; set; }
    public double FuelConsumptionLiters { get; set; }
    public TollRoadsStatus TollRoadsStatus { get; set; }
    public RouteProvider Provider { get; set; }
    public bool IsEstimated { get; set; }
    public bool FromCache { get; set; }
    public List<ResolvedRoutePointResponse> ResolvedPoints { get; set; } = new();
    public RouteGeometryResponse Geometry { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}
