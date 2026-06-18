namespace ListingService.Domain.Models;

public class RoutePoint
{
    public Guid Id { get; set; }
    public Guid? CargoId { get; set; }
    public Guid? TruckId { get; set; }
    public string Address { get; set; } = string.Empty;
    public double Lat { get; set; }
    public double Lon { get; set; }
    public DateTime? ScheduledTime { get; set; }
    public int Order { get; set; }
}
