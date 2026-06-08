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
        cargo.Id = Guid.NewGuid();
        return await _cargosRepository.Create(cargo);
    }

    public async Task<Guid> UpdateCargoAsync(Guid id, Cargo cargo)
    {
        return await _cargosRepository.Update(id, cargo);
    }

    public async Task<Guid> DeleteCargoAsync(Guid id)
    {
        return await _cargosRepository.Delete(id);
    }
}