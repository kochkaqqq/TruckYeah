using ListingService.Domain.Models;

namespace ListingService.Infrastructure.Interfaces;

public interface ITruckRepository
{
    Task<List<Truck>> Get();
    
    Task<Guid> Create(Truck truck);
    
    Task<Guid> Update(Guid id, string title, string description, string bodyType, 
        double? capacityKg, double? volumeM3,  double? lengthCm, 
        double? widthCm, double? heightCm,  string currentLocation, 
        string routeFrom, string routeTo, int? radiusKm, 
        decimal? pricePerKm,  string currency);
    
    Task<Guid> Delete(Guid id);
}