using RouteService.Domain.Enums;

namespace RouteService.Domain.Models;

public class RouteCalculation
{
    public Guid Id { get; set; }
    public RouteProvider Provider { get; set; }
    public string RequestHash { get; set; } = string.Empty;
    public double DistanceKm { get; set; }
    public int DurationMinutes { get; set; }
    public double FuelConsumptionLiters { get; set; }
    public TollRoadsStatus TollRoadsStatus { get; set; }
    public string? Geometry { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }

    public ICollection<ResolvedRoutePoint> ResolvedPoints { get; set; } = new List<ResolvedRoutePoint>();
}
