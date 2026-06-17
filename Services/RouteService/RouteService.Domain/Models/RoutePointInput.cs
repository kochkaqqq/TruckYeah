namespace RouteService.Domain.Models;

public class RoutePointInput
{
    public string? Address { get; set; }
    public double? Lat { get; set; }
    public double? Lon { get; set; }
    public int Order { get; set; }
}
