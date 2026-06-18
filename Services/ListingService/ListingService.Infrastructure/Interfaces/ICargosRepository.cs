using ListingService.Domain.Models;

namespace ListingService.Infrastructure.Interfaces;

public interface ICargosRepository
{
    Task<List<Cargo>> Search(CargoSearchCriteria criteria, bool publishedOnly, Guid? userId = null);
    Task<Cargo?> GetById(Guid id);
    Task<Guid> Create(Cargo cargo);
    Task Update(Cargo cargo);
    Task Delete(Guid id);
    Task<List<CargoBid>> GetBids(Guid cargoId);
    Task<List<CargoBid>> GetBidsByCarrier(Guid carrierUserId);
    Task<Guid> CreateBid(CargoBid bid);
    Task UpdateBids(IEnumerable<CargoBid> bids);
}
