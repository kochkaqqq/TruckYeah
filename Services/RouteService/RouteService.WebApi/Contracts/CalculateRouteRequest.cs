namespace RouteService.WebApi.Contracts;

public class CalculateRouteRequest
{
    public List<RoutePointInputRequest> Points { get; set; } = new();
    public double? FuelConsumptionLitersPer100Km { get; set; }
    public RouteVehicleOptionsRequest? Vehicle { get; set; }
    public bool AvoidTollRoads { get; set; }
    public bool AvoidFerries { get; set; }
}
