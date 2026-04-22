using ListingService.Domain.Models;

namespace ListingService.Infrastructure.Interfaces;

public interface ICargosRepository
{
    Task<List<Cargo>> Get();
    
    Task<Guid> Create(Cargo cargo);
    
    Task<Guid> Update(Guid id, string title, string description, 
        double? weightKg, double? volumeM3, double? lengthCm, 
        double? widthCm, double? heightCm, string cargoType, 
        string? routeFrom, string? routeTo, double? distanceKm, 
        DateTime loadDate, decimal? price);
    
    Task<Guid> Delete(Guid id);
}