using ListingService.Domain.Models;

namespace ListingService.Application.Interfaces;

public interface ICargoService
{
    Task<List<Cargo>> GetAllCargosAsync();
    
    Task<Guid> CreateCargoAsync(Cargo cargo);
    
    Task<Guid> UpdateCargoAsync(Guid id, string title, string description, 
        double? weightKg, double? volumeM3, double? lengthCm, 
        double? widthCm, double? heightCm, string cargoType, 
        string? routeFrom, string? routeTo, double? distanceKm, 
        DateTime loadDate, decimal? price);
    
    Task<Guid> DeleteCargoAsync(Guid id);
}