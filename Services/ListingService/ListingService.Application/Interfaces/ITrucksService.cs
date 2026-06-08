using ListingService.Domain.Models;

namespace ListingService.Application.Interfaces;

public interface ITrucksService
{
    Task<List<Truck>> GetTrucksAsync();
    
    Task<Guid> CreateTruckAsync(Truck truck);
    
    Task<Guid> UpdateTruckAsync(Guid id, Truck truck);
    
    Task<Guid> DeleteTruckAsync(Guid id);
}