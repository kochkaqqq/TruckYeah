namespace RouteService.WebApi.Contracts;

public class RoutePointInputRequest
{
    public string? Address { get; set; }
    public double? Lat { get; set; }
    public double? Lon { get; set; }
    public int Order { get; set; }
}
