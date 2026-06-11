namespace ListingService.Infrastructure.Entities;

public class CargoBidEntity
{
    public Guid Id { get; set; }
    public Guid CargoId { get; set; }
    public Guid CarrierUserId { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }

    public CargoEntity? Cargo { get; set; }
}
