using ListingService.Domain.Models;

namespace ListingService.Application.Interfaces;

public interface ICargosService
{
    Task<List<Cargo>> SearchPublishedCargosAsync(CargoSearchCriteria criteria);
    Task<List<Cargo>> GetMyCargosAsync(Guid userId);
    Task<Cargo> GetCargoByIdAsync(Guid id);
    Task<Guid> CreateCargoAsync(Cargo cargo);
    Task<Guid> UpdateCargoAsync(Guid id, Guid userId, Cargo cargo);
    Task<Guid> DeleteCargoAsync(Guid id, Guid userId);
    Task<Guid> PublishCargoAsync(Guid id, Guid userId);
    Task<Guid> ArchiveCargoAsync(Guid id, Guid userId);
    Task<Guid> CopyCargoAsync(Guid id, Guid userId);
    Task<Guid> SaveCargoTemplateAsync(Guid id, Guid userId, string? templateName);
    Task<List<CargoBid>> GetCargoBidsAsync(Guid cargoId, Guid ownerUserId);
    Task<Guid> CreateCargoBidAsync(Guid cargoId, Guid carrierUserId, decimal price);
}
