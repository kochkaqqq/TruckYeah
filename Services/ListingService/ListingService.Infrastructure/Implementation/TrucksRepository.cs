using ListingService.Domain.Enums;
using ListingService.Domain.Models;
using ListingService.Infrastructure.Entities;
using ListingService.Infrastructure.Interfaces;
using ListingService.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace ListingService.Infrastructure.Implementation;

public class TrucksRepository : ITrucksRepository
{
    private readonly ListingServiceDbContext _dbContext;

    public TrucksRepository(ListingServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Truck>> Search(TruckSearchCriteria criteria, bool publishedOnly, Guid? userId = null)
    {
        var query = _dbContext.Trucks.AsNoTracking().AsQueryable();

        if (publishedOnly)
        {
            query = query.Where(t => t.Status == ListingStatus.Published);
        }

        if (userId.HasValue)
        {
            query = query.Where(t => t.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.RouteFrom))
        {
            query = query.Where(t => EF.Functions.ILike(t.RouteFrom, $"%{criteria.RouteFrom}%"));
        }

        if (!string.IsNullOrWhiteSpace(criteria.RouteTo))
        {
            query = query.Where(t => EF.Functions.ILike(t.RouteTo, $"%{criteria.RouteTo}%"));
        }

        if (criteria.AvailableDate.HasValue)
        {
            var date = criteria.AvailableDate.Value.Date;
            var nextDate = date.AddDays(1);
            query = query.Where(t => t.AvailableFrom >= date && t.AvailableFrom < nextDate);
        }

        if (criteria.CapacityFrom.HasValue)
        {
            query = query.Where(t => t.CapacityTons >= criteria.CapacityFrom.Value);
        }

        if (criteria.CapacityTo.HasValue)
        {
            query = query.Where(t => t.CapacityTons <= criteria.CapacityTo.Value);
        }

        if (criteria.VolumeFrom.HasValue)
        {
            query = query.Where(t => t.VolumeM3 >= criteria.VolumeFrom.Value);
        }

        if (criteria.VolumeTo.HasValue)
        {
            query = query.Where(t => t.VolumeM3 <= criteria.VolumeTo.Value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.BodyType))
        {
            query = query.Where(t => EF.Functions.ILike(t.BodyType, $"%{criteria.BodyType}%"));
        }

        if (criteria.LoadingType.HasValue)
        {
            query = query.Where(t => t.LoadingType == criteria.LoadingType.Value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.AdditionalEquipment))
        {
            query = query.Where(t => t.AdditionalEquipment != null &&
                                     EF.Functions.ILike(t.AdditionalEquipment, $"%{criteria.AdditionalEquipment}%"));
        }

        var entities = await ApplyPaging(ApplySorting(query, criteria), criteria.Page, criteria.PageSize)
            .ToListAsync();

        return entities.Select(t => t.MapToModel()).ToList();
    }

    public async Task<Truck?> GetById(Guid id)
    {
        var entity = await _dbContext.Trucks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);

        return entity?.MapToModel();
    }

    public async Task<Guid> Create(Truck truck)
    {
        var entity = truck.MapToEntity();
        await _dbContext.Trucks.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entity.Id;
    }

    public async Task Update(Truck truck)
    {
        var entity = await _dbContext.Trucks.FirstOrDefaultAsync(t => t.Id == truck.Id);

        if (entity is null)
        {
            throw new KeyNotFoundException("Truck was not found.");
        }

        ApplyTruck(entity, truck);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var affectedRows = await _dbContext.Trucks
            .Where(t => t.Id == id)
            .ExecuteDeleteAsync();

        if (affectedRows == 0)
        {
            throw new KeyNotFoundException("Truck was not found.");
        }
    }

    private static void ApplyTruck(TruckEntity entity, Truck truck)
    {
        entity.Title = truck.Title;
        entity.Description = truck.Description;
        entity.RouteFrom = truck.RouteFrom;
        entity.RouteTo = truck.RouteTo;
        entity.CapacityTons = truck.CapacityTons;
        entity.VolumeM3 = truck.VolumeM3;
        entity.BodyType = truck.BodyType;
        entity.LoadingType = truck.LoadingType;
        entity.CrewDriversCount = truck.CrewDriversCount;
        entity.AdditionalEquipment = truck.AdditionalEquipment;
        entity.AvailableFrom = truck.AvailableFrom;
        entity.Price = truck.Price;
        entity.PaymentType = truck.PaymentType;
        entity.AllowBargaining = truck.AllowBargaining;
        entity.PrepaymentPercent = truck.PrepaymentPercent;
        entity.Status = truck.Status;
        entity.Visibility = truck.Visibility;
        entity.PublishedAt = truck.PublishedAt;
        entity.SourceListingId = truck.SourceListingId;
    }

    private static IOrderedQueryable<TruckEntity> ApplySorting(IQueryable<TruckEntity> query, TruckSearchCriteria criteria)
    {
        var descending = !string.Equals(criteria.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);

        return criteria.SortBy?.Trim().ToLowerInvariant() switch
        {
            "price" => descending
                ? query.OrderByDescending(t => t.Price)
                : query.OrderBy(t => t.Price),
            "availabledate" or "availablefrom" => descending
                ? query.OrderByDescending(t => t.AvailableFrom)
                : query.OrderBy(t => t.AvailableFrom),
            "capacity" or "capacitytons" => descending
                ? query.OrderByDescending(t => t.CapacityTons)
                : query.OrderBy(t => t.CapacityTons),
            "volume" or "volumem3" => descending
                ? query.OrderByDescending(t => t.VolumeM3)
                : query.OrderBy(t => t.VolumeM3),
            "createdat" => descending
                ? query.OrderByDescending(t => t.CreatedAt)
                : query.OrderBy(t => t.CreatedAt),
            _ => descending
                ? query.OrderByDescending(t => t.PublishedAt ?? t.CreatedAt)
                : query.OrderBy(t => t.PublishedAt ?? t.CreatedAt)
        };
    }

    private static IQueryable<T> ApplyPaging<T>(IQueryable<T> query, int? page, int? pageSize)
    {
        if (!page.HasValue && !pageSize.HasValue)
        {
            return query;
        }

        var normalizedPage = Math.Max(page.GetValueOrDefault(1), 1);
        var normalizedPageSize = Math.Clamp(pageSize.GetValueOrDefault(50), 1, 100);

        return query
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize);
    }
}
