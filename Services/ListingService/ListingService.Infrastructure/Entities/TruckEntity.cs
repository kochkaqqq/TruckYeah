using ListingService.Domain.Enums;

namespace ListingService.Infrastructure.Entities;

public class TruckEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public string RouteFrom { get; set; } = string.Empty;
    public string RouteTo { get; set; } = string.Empty;

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

    public ListingStatus Status { get; set; }
    public ListingVisibility Visibility { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public Guid? SourceListingId { get; set; }
}
