namespace ListingService.WebApi.Contracts.Cargo;

public class RoutePointRequest
{
    public string Address { get; set; } = string.Empty;
    public DateTime? ScheduledTime { get; set; }
    public int Order { get; set; }
}
