using ListingService.Domain.Models;

namespace ListingService.Application.Interfaces;

public interface ITrucksService
{
    Task<List<Truck>> SearchPublishedTrucksAsync(TruckSearchCriteria criteria);
    Task<List<Truck>> GetMyTrucksAsync(Guid userId);
    Task<Truck> GetTruckByIdAsync(Guid id);
    Task<Guid> CreateTruckAsync(Truck truck);
    Task<Guid> UpdateTruckAsync(Guid id, Guid userId, Truck truck);
    Task<Guid> DeleteTruckAsync(Guid id, Guid userId);
    Task<Guid> PublishTruckAsync(Guid id, Guid userId);
    Task<Guid> ArchiveTruckAsync(Guid id, Guid userId);
    Task<Guid> CopyTruckAsync(Guid id, Guid userId);
    Task<List<Truck>> GetAllForModerationAsync();
    Task<Guid> ApproveAsync(Guid id, Guid moderatorId);
    Task<Guid> RejectAsync(Guid id, Guid moderatorId, string reason);
}
