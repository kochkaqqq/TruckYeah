using ListingService.Domain.Enums;

namespace ListingService.WebApi.Contracts.Cargo;

public class CargoResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CargoName { get; set; } = string.Empty;
    public string RouteFrom { get; set; } = string.Empty;
    public string RouteTo { get; set; } = string.Empty;
    public List<RoutePointResponse> RoutePoints { get; set; } = new();
    public double? RouteDistanceKm { get; set; }
    public int? RouteDurationMinutes { get; set; }
    public double? RouteFuelLiters { get; set; }
    public string? RouteGeometryGeoJson { get; set; }
    public DateTime? RouteCalculatedAt { get; set; }
    public DateTime LoadDateTime { get; set; }
    public DateTime UnloadDateTime { get; set; }
    public double WeightTons { get; set; }
    public double VolumeM3 { get; set; }
    public bool UseAutomaticCalculation { get; set; }
    public double? WeightPerPackageKg { get; set; }
    public string BodyTypeRequired { get; set; } = string.Empty;
    public LoadingType LoadingType { get; set; }
    public double? LengthCm { get; set; }
    public double? WidthCm { get; set; }
    public double? HeightCm { get; set; }
    public int? PalletsCount { get; set; }
    public string? PackagingType { get; set; }
    public bool RequiresCMR { get; set; }
    public bool RequiresTIR { get; set; }
    public bool IsADR { get; set; }
    public bool RequiresTwoDrivers { get; set; }
    public PaymentType PaymentType { get; set; }
    public bool AllowBargaining { get; set; }
    public decimal? PrepaymentPercent { get; set; }
    public decimal? StartingPrice { get; set; }
    public bool BiddingEnabled { get; set; }
    public decimal? MinBidStep { get; set; }
    public Guid? AcceptedBidId { get; set; }
    public DateTime? BiddingClosedAt { get; set; }
    public ListingStatus Status { get; set; }
    public ListingVisibility Visibility { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public bool BoostToTop { get; set; }
    public DateTime? BoostedUntil { get; set; }
    public bool IsTemplate { get; set; }
    public string? TemplateName { get; set; }
    public Guid? SourceListingId { get; set; }
    public string? Notes { get; set; }
}
