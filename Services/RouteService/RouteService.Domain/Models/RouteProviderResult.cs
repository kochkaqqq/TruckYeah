using RouteService.Domain.Enums;

namespace RouteService.Domain.Models;

public class RouteProviderResult
{
    public double DistanceKm { get; set; }
    public int DurationMinutes { get; set; }
    public TollRoadsStatus TollRoadsStatus { get; set; }
    public RouteGeometry Geometry { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}
