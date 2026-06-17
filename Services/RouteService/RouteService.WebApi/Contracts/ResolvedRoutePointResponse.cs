namespace RouteService.WebApi.Contracts;

public class ResolvedRoutePointResponse
{
    public string Address { get; set; } = string.Empty;
    public double Lat { get; set; }
    public double Lon { get; set; }
    public int Order { get; set; }
}
