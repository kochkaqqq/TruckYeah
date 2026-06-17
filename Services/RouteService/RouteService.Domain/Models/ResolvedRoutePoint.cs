namespace RouteService.Domain.Models;

public class ResolvedRoutePoint
{
    public Guid Id { get; set; }
    public Guid RouteCalculationId { get; set; }
    public string Address { get; set; } = string.Empty;
    public double Lat { get; set; }
    public double Lon { get; set; }
    public int Order { get; set; }
}
