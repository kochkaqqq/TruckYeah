using ListingService.Domain.Enums;

namespace ListingService.WebApi.Contracts.Cargo;

public class CargoSearchQuery
{
    public string? RouteFrom { get; set; }
    public string? RouteTo { get; set; }
    public DateTime? LoadDate { get; set; }
    public double? WeightFrom { get; set; }
    public double? WeightTo { get; set; }
    public double? VolumeFrom { get; set; }
    public double? VolumeTo { get; set; }
    public string? BodyType { get; set; }
    public string? CargoName { get; set; }
    public LoadingType? LoadingType { get; set; }
    public bool? OnlyWithBidding { get; set; }
    public ListingVisibility? Visibility { get; set; }
}
