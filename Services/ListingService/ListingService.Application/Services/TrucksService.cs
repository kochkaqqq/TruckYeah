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

    public async Task<Guid> UpdateTruckAsync(Guid id, Truck truck)
    {
        return await _repository.Update(id, truck);
    }

    public async Task<Guid> DeleteTruckAsync(Guid id)
    {
        return await _repository.Delete(id);
    }
}