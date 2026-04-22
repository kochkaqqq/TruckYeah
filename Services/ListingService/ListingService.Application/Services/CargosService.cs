using ListingService.Application.Interfaces;
using ListingService.Domain.Models;
using ListingService.Infrastructure.Interfaces;

namespace ListingService.Application.Services;

public class CargosService : ICargosService
{
    private readonly ICargosRepository _cargosRepository;

    public CargosService(ICargosRepository cargosRepository)
    {
        _cargosRepository = cargosRepository;
    }
    
    public async Task<List<Cargo>> GetAllCargosAsync()
    {
        return await _cargosRepository.Get();
    }

    public async Task<Guid> CreateCargoAsync(Cargo cargo)
    {
        return await _cargosRepository.Create(cargo);
    }

    public async Task<Guid> UpdateCargoAsync(Guid id, string title, string description, double? weightKg, double? volumeM3, double? lengthCm,
        double? widthCm, double? heightCm, string cargoType, string? routeFrom, string? routeTo, double? distanceKm,
        DateTime loadDate, decimal? price)
    {
        return await UpdateCargoAsync(id, title, description, weightKg, 
            volumeM3, lengthCm, widthCm, heightCm, 
            cargoType, routeFrom, routeTo, 
            distanceKm, loadDate, price);
    }

    public async Task<Guid> DeleteCargoAsync(Guid id)
    {
        return await _cargosRepository.Delete(id);
    }
}