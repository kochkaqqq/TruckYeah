namespace RouteService.Domain.Models;

public class RouteVehicleOptions
{
    public double? HeightMeters { get; set; }
    public double? WidthMeters { get; set; }
    public double? LengthMeters { get; set; }
    public double? WeightTons { get; set; }
    public double? AxleLoadTons { get; set; }
    public bool Hazmat { get; set; }
}
