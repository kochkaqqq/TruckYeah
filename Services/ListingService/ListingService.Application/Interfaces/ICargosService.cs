using ListingService.Domain.Models;

namespace ListingService.Application.Interfaces;

public interface ICargosService
{
    Task<List<Cargo>> GetAllCargosAsync();
    
    Task<Guid> CreateCargoAsync(Cargo cargo);
    
    Task<Guid> UpdateCargoAsync(Guid id, Cargo cargo);
    
    Task<Guid> DeleteCargoAsync(Guid id);
}