using ListingService.Application.Interfaces;
using ListingService.Domain.Enums;
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

    public Task<List<Truck>> SearchPublishedTrucksAsync(TruckSearchCriteria criteria)
    {
        return _repository.Search(criteria, publishedOnly: true);
    }

    public Task<List<Truck>> GetMyTrucksAsync(Guid userId)
    {
        return _repository.Search(new TruckSearchCriteria(), publishedOnly: false, userId);
    }

    public async Task<Truck> GetTruckByIdAsync(Guid id)
    {
        return await GetExistingTruck(id);
    }

    public async Task<Guid> CreateTruckAsync(Truck truck)
    {
        ValidateTruck(truck);
        truck.Id = Guid.NewGuid();
        truck.CreatedAt = DateTime.UtcNow;
        truck.Status = ListingStatus.Draft;
        truck.PublishedAt = null;
        return await _repository.Create(truck);
    }

    public async Task<Guid> UpdateTruckAsync(Guid id, Guid userId, Truck truck)
    {
        ValidateTruck(truck);

        var existing = await GetExistingTruck(id);
        EnsureOwner(existing.UserId, userId);

        truck.Id = existing.Id;
        truck.UserId = existing.UserId;
        truck.CreatedAt = existing.CreatedAt;
        truck.Status = existing.Status;
        truck.PublishedAt = existing.PublishedAt;
        truck.SourceListingId = existing.SourceListingId;

        await _repository.Update(truck);
        return id;
    }

    public async Task<Guid> DeleteTruckAsync(Guid id, Guid userId)
    {
        var existing = await GetExistingTruck(id);
        EnsureOwner(existing.UserId, userId);
        await _repository.Delete(id);
        return id;
    }

    public async Task<Guid> PublishTruckAsync(Guid id, Guid userId)
    {
        var truck = await GetExistingTruck(id);
        EnsureOwner(truck.UserId, userId);
        ValidateTruck(truck);

        truck.Status = ListingStatus.Published;
        truck.PublishedAt = DateTime.UtcNow;
        await _repository.Update(truck);
        return id;
    }

    public async Task<Guid> ArchiveTruckAsync(Guid id, Guid userId)
    {
        var truck = await GetExistingTruck(id);
        EnsureOwner(truck.UserId, userId);

        truck.Status = ListingStatus.Archived;
        await _repository.Update(truck);
        return id;
    }

    public async Task<Guid> CopyTruckAsync(Guid id, Guid userId)
    {
        var source = await GetExistingTruck(id);
        EnsureOwner(source.UserId, userId);

        var copy = new Truck
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = source.Title,
            Description = source.Description,
            RouteFrom = source.RouteFrom,
            RouteTo = source.RouteTo,
            CapacityTons = source.CapacityTons,
            VolumeM3 = source.VolumeM3,
            BodyType = source.BodyType,
            LoadingType = source.LoadingType,
            CrewDriversCount = source.CrewDriversCount,
            AdditionalEquipment = source.AdditionalEquipment,
            AvailableFrom = source.AvailableFrom,
            Price = source.Price,
            PaymentType = source.PaymentType,
            AllowBargaining = source.AllowBargaining,
            PrepaymentPercent = source.PrepaymentPercent,
            Visibility = source.Visibility,
            Status = ListingStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            SourceListingId = source.Id
        };

        return await _repository.Create(copy);
    }

    private async Task<Truck> GetExistingTruck(Guid id)
    {
        return await _repository.GetById(id)
            ?? throw new KeyNotFoundException("Truck was not found.");
    }

    private static void ValidateTruck(Truck truck)
    {
        if (string.IsNullOrWhiteSpace(truck.Title))
            throw new ArgumentException("Title is required.");
        if (string.IsNullOrWhiteSpace(truck.RouteFrom))
            throw new ArgumentException("Route from is required.");
        if (string.IsNullOrWhiteSpace(truck.RouteTo))
            throw new ArgumentException("Route to is required.");
        if (truck.CapacityTons <= 0)
            throw new ArgumentException("Capacity must be greater than zero.");
        if (truck.VolumeM3 <= 0)
            throw new ArgumentException("Volume must be greater than zero.");
        if (string.IsNullOrWhiteSpace(truck.BodyType))
            throw new ArgumentException("Body type is required.");
        if (truck.AvailableFrom == default)
            throw new ArgumentException("Available from date is required.");
        if (truck.Price <= 0)
            throw new ArgumentException("Price must be greater than zero.");
        if (truck.CrewDriversCount is < 1 or > 2)
            throw new ArgumentException("Crew drivers count must be 1 or 2.");
        if (truck.PrepaymentPercent is < 0 or > 100)
            throw new ArgumentException("Prepayment percent must be from 0 to 100.");
        if (truck.AdditionalEquipment?.Length > 1000)
            throw new ArgumentException("Additional equipment description cannot be longer than 1000 characters.");
    }

    private static void EnsureOwner(Guid ownerUserId, Guid currentUserId)
    {
        if (ownerUserId != currentUserId)
        {
            throw new UnauthorizedAccessException("Only listing owner can perform this action.");
        }
    }
}
