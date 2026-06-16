using ListingService.Domain.Enums;

namespace ListingService.Domain.Models;

public class TruckSearchCriteria
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
    public string? AdditionalEquipment { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
}
