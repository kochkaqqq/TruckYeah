using ListingService.Application.Interfaces;
using ListingService.Domain.Models;
using ListingService.Infrastructure.Interfaces;

namespace ListingService.Application.Services;

public class TrucksService : ITrucksService
{
    private readonly ITrucksRepository _repository;
    
    public TrucksService(ITrucksRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<List<Truck>> GetTrucksAsync()
    {
        return await _repository.Get();
    }

    public async Task<Guid> CreateTruckAsync(Truck truck)
    {
        return await _repository.Create(truck);
    }

    public async Task<Guid> UpdateTruckAsync(Guid id, string title, string description, string bodyType, double? capacityKg, double? volumeM3,
        double? lengthCm, double? widthCm, double? heightCm, string currentLocation, string routeFrom, string routeTo,
        int? radiusKm, decimal? pricePerKm, string currency)
    {
        return await _repository.Update(id, title, description, bodyType, 
            capacityKg, volumeM3, lengthCm, widthCm,
            heightCm, currentLocation, routeFrom, routeTo, 
            radiusKm, pricePerKm, currency);
    }

    public async Task<Guid> DeleteTruckAsync(Guid id)
    {
        return await _repository.Delete(id);
    }
}