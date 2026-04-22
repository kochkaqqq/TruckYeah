using ListingService.Domain.Models;

namespace ListingService.Application.Interfaces;

public interface ITrucksService
{
    Task<List<Truck>> GetTrucksAsync();
    
    Task<Guid> CreateTruckAsync(Truck truck);
    
    Task<Guid> UpdateTruckAsync(Guid id, string title, string description, string bodyType, 
        double? capacityKg, double? volumeM3,  double? lengthCm, 
        double? widthCm, double? heightCm,  string currentLocation, 
        string routeFrom, string routeTo, int? radiusKm, 
        decimal? pricePerKm,  string currency);
    
    Task<Guid> DeleteTruckAsync(Guid id);
}