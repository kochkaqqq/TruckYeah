namespace ListingService.Domain.Models;

public class RoutePoint
{
    public Guid Id { get; set; }
    public Guid CargoId { get; set; }
    public string Address { get; set; } = string.Empty;  // Адрес точки
    public DateTime? ScheduledTime { get; set; }         // Плановое время прибытия
    public int Order { get; set; }                       // Порядок в маршруте
}