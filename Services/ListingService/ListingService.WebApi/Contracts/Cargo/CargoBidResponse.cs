using ListingService.Domain.Enums;

namespace ListingService.WebApi.Contracts.Cargo;

public class CargoBidResponse
{
    public Guid Id { get; set; }
    public Guid CargoId { get; set; }
    public Guid CarrierUserId { get; set; }
    public decimal Price { get; set; }
    public BidStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
}
