using ListingService.Domain.Models;

namespace ListingService.Infrastructure.Interfaces;

public interface ITrucksRepository
{
    Task<List<Truck>> Search(TruckSearchCriteria criteria, bool publishedOnly, Guid? userId = null);
    Task<Truck?> GetById(Guid id);
    Task<Guid> Create(Truck truck);
    Task Update(Truck truck);
    Task Delete(Guid id);
    Task<List<Truck>> GetAll();
}
