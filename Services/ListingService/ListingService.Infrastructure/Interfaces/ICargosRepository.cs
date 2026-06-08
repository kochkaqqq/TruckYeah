using ListingService.Domain.Models;

namespace ListingService.Infrastructure.Interfaces;

public interface ICargosRepository
{
    Task<List<Cargo>> Get();
    
    Task<Guid> Create(Cargo cargo);
    
    Task<Guid> Update(Guid id, Cargo cargo);
    
    Task<Guid> Delete(Guid id);
}