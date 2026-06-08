using ListingService.Domain.Models;

namespace ListingService.Infrastructure.Interfaces;

public interface ITrucksRepository
{
    Task<List<Truck>> Get();
    
    Task<Guid> Create(Truck truck);
    
    Task<Guid> Update(Guid id, Truck truck);
    
    Task<Guid> Delete(Guid id);
}