namespace ListingService.WebApi.Contracts.Cargo;

public class RoutePointResponse
{
    public Guid Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public double Lat { get; set; }
    public double Lon { get; set; }
    public DateTime? ScheduledTime { get; set; }
    public int Order { get; set; }
}
