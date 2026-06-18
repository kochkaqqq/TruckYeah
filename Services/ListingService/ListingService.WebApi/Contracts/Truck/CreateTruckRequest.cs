using ListingService.Domain.Enums;
using ListingService.WebApi.Contracts.Cargo;

namespace ListingService.WebApi.Contracts.Truck;

public class CreateTruckRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string RouteFrom { get; set; } = string.Empty;
    public string RouteTo { get; set; } = string.Empty;
    public List<RoutePointRequest> RoutePoints { get; set; } = new();
    public double? RouteDistanceKm { get; set; }
    public int? RouteDurationMinutes { get; set; }
    public double? RouteFuelLiters { get; set; }
    public string? RouteGeometryGeoJson { get; set; }
    public DateTime? RouteCalculatedAt { get; set; }
    public double CapacityTons { get; set; }
    public double VolumeM3 { get; set; }
    public string BodyType { get; set; } = string.Empty;
    public LoadingType LoadingType { get; set; }
    public int CrewDriversCount { get; set; } = 1;
    public string? AdditionalEquipment { get; set; }
    public DateTime AvailableFrom { get; set; }
    public decimal Price { get; set; }
    public PaymentType PaymentType { get; set; }
    public bool AllowBargaining { get; set; }
    public decimal? PrepaymentPercent { get; set; }
    public ListingVisibility Visibility { get; set; } = ListingVisibility.Exchange;
}
