using ListingService.Domain.Models;
using ListingService.Infrastructure.Entities;

namespace ListingService.Infrastructure.Mappers;

public static class CargoBidMapper
{
    public static CargoBid MapToModel(this CargoBidEntity entity)
    {
        return new CargoBid
        {
            Id = entity.Id,
            CargoId = entity.CargoId,
            CarrierUserId = entity.CarrierUserId,
            Price = entity.Price,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            AcceptedAt = entity.AcceptedAt
        };
    }

    public static CargoBidEntity MapToEntity(this CargoBid bid)
    {
        return new CargoBidEntity
        {
            Id = bid.Id,
            CargoId = bid.CargoId,
            CarrierUserId = bid.CarrierUserId,
            Price = bid.Price,
            Status = bid.Status,
            CreatedAt = bid.CreatedAt,
            AcceptedAt = bid.AcceptedAt
        };
    }
}
