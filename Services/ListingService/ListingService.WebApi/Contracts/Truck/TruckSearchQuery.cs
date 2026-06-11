using ListingService.Domain.Enums;

namespace ListingService.WebApi.Contracts.Truck;

public class TruckSearchQuery
{
    public string? RouteFrom { get; set; }
    public string? RouteTo { get; set; }
    public DateTime? AvailableDate { get; set; }
    public double? CapacityFrom { get; set; }
    public double? CapacityTo { get; set; }
    public double? VolumeFrom { get; set; }
    public double? VolumeTo { get; set; }
    public string? BodyType { get; set; }
    public LoadingType? LoadingType { get; set; }
}
