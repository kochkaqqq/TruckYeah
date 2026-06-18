using ListingService.Domain.Enums;
using ListingService.Domain.Models;
using ListingService.Infrastructure.Entities;
using ListingService.Infrastructure.Interfaces;
using ListingService.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace ListingService.Infrastructure.Implementation;

public class CargosRepository : ICargosRepository
{
    private readonly ListingServiceDbContext _dbContext;

    public CargosRepository(ListingServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Cargo>> Search(CargoSearchCriteria criteria, bool publishedOnly, Guid? userId = null)
    {
        var query = _dbContext.Cargos
            .Include(c => c.RoutePoints)
            .AsNoTracking()
            .AsQueryable();

        if (publishedOnly)
        {
            query = query.Where(c => c.Status == ListingStatus.Published && !c.IsTemplate);
        }

        if (userId.HasValue)
        {
            query = query.Where(c => c.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.RouteFrom))
        {
            query = query.Where(c => EF.Functions.ILike(c.RouteFrom, $"%{criteria.RouteFrom}%"));
        }

        if (!string.IsNullOrWhiteSpace(criteria.RouteTo))
        {
            query = query.Where(c => EF.Functions.ILike(c.RouteTo, $"%{criteria.RouteTo}%"));
        }

        if (criteria.LoadDate.HasValue)
        {
            var date = criteria.LoadDate.Value.Date;
            var nextDate = date.AddDays(1);
            query = query.Where(c => c.LoadDateTime >= date && c.LoadDateTime < nextDate);
        }

        if (criteria.WeightFrom.HasValue)
        {
            query = query.Where(c => c.WeightTons >= criteria.WeightFrom.Value);
        }

        if (criteria.WeightTo.HasValue)
        {
            query = query.Where(c => c.WeightTons <= criteria.WeightTo.Value);
        }

        if (criteria.VolumeFrom.HasValue)
        {
            query = query.Where(c => c.VolumeM3 >= criteria.VolumeFrom.Value);
        }

        if (criteria.VolumeTo.HasValue)
        {
            query = query.Where(c => c.VolumeM3 <= criteria.VolumeTo.Value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.BodyType))
        {
            query = query.Where(c => EF.Functions.ILike(c.BodyTypeRequired, $"%{criteria.BodyType}%"));
        }

        if (!string.IsNullOrWhiteSpace(criteria.CargoName))
        {
            query = query.Where(c => EF.Functions.ILike(c.CargoName, $"%{criteria.CargoName}%"));
        }

        if (criteria.LoadingType.HasValue)
        {
            query = query.Where(c => c.LoadingType == criteria.LoadingType.Value);
        }

        if (criteria.OnlyWithBidding == true)
        {
            query = query.Where(c => c.BiddingEnabled);
        }

        if (criteria.Visibility.HasValue)
        {
            query = query.Where(c => c.Visibility == criteria.Visibility.Value);
        }

        var entities = await ApplyPaging(ApplySorting(query, criteria), criteria.Page, criteria.PageSize)
            .ToListAsync();

        return entities.Select(c => c.MapToModel()).ToList();
    }

    public async Task<Cargo?> GetById(Guid id)
    {
        var entity = await _dbContext.Cargos
            .Include(c => c.RoutePoints)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        return entity?.MapToModel();
    }

    public async Task<List<Cargo>> GetAll()
    {
        var entities = await _dbContext.Cargos
            .Include(c => c.RoutePoints)
            .AsNoTracking()
            .Where(c => !c.IsTemplate)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return entities.Select(c => c.MapToModel()).ToList();
    }

    public async Task<Guid> Create(Cargo cargo)
    {
        var entity = cargo.MapToEntity();
        await _dbContext.Cargos.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entity.Id;
    }

    public async Task Update(Cargo cargo)
    {
        var entity = await _dbContext.Cargos
            .FirstOrDefaultAsync(c => c.Id == cargo.Id);

        if (entity is null)
        {
            throw new KeyNotFoundException("Cargo was not found.");
        }

        ApplyCargo(entity, cargo);

        await _dbContext.RoutePoints
            .Where(p => p.CargoId == cargo.Id)
            .ExecuteDeleteAsync();

        var routePoints = cargo.RoutePoints
            .OrderBy(p => p.Order)
            .Select(p => new RoutePoint
            {
                Id = p.Id == Guid.Empty ? Guid.NewGuid() : p.Id,
                CargoId = cargo.Id,
                Address = p.Address,
                Lat = p.Lat,
                Lon = p.Lon,
                ScheduledTime = p.ScheduledTime,
                Order = p.Order
            })
            .ToList();

        await _dbContext.RoutePoints.AddRangeAsync(routePoints);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var affectedRows = await _dbContext.Cargos
            .Where(c => c.Id == id)
            .ExecuteDeleteAsync();

        if (affectedRows == 0)
        {
            throw new KeyNotFoundException("Cargo was not found.");
        }
    }

    public async Task<List<CargoBid>> GetBids(Guid cargoId)
    {
        var bids = await _dbContext.CargoBids
            .AsNoTracking()
            .Where(b => b.CargoId == cargoId)
            .OrderBy(b => b.Price)
            .ThenBy(b => b.CreatedAt)
            .ToListAsync();

        return bids.Select(b => b.MapToModel()).ToList();
    }

    public async Task<List<CargoBid>> GetBidsByCarrier(Guid carrierUserId)
    {
        return await _dbContext.CargoBids
            .AsNoTracking()
            .Where(b => b.CarrierUserId == carrierUserId)
            .OrderByDescending(b => b.CreatedAt)
            .Select(b => new CargoBid
            {
                Id = b.Id,
                CargoId = b.CargoId,
                CarrierUserId = b.CarrierUserId,
                Price = b.Price,
                Status = b.Status,
                CreatedAt = b.CreatedAt,
                AcceptedAt = b.AcceptedAt
            })
            .ToListAsync();
    }

    public async Task<Guid> CreateBid(CargoBid bid)
    {
        var entity = bid.MapToEntity();
        await _dbContext.CargoBids.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entity.Id;
    }

    public async Task UpdateBids(IEnumerable<CargoBid> bids)
    {
        var bidList = bids.ToList();
        var bidIds = bidList.Select(b => b.Id).ToList();
        var entities = await _dbContext.CargoBids
            .Where(b => bidIds.Contains(b.Id))
            .ToListAsync();

        foreach (var entity in entities)
        {
            var bid = bidList.First(b => b.Id == entity.Id);
            entity.Status = bid.Status;
            entity.AcceptedAt = bid.AcceptedAt;
        }

        await _dbContext.SaveChangesAsync();
    }

    private static void ApplyCargo(CargoEntity entity, Cargo cargo)
    {
        entity.Title = cargo.Title;
        entity.CargoName = cargo.CargoName;
        entity.RouteFrom = cargo.RouteFrom;
        entity.RouteTo = cargo.RouteTo;
        entity.RouteDistanceKm = cargo.RouteDistanceKm;
        entity.RouteDurationMinutes = cargo.RouteDurationMinutes;
        entity.RouteFuelLiters = cargo.RouteFuelLiters;
        entity.RouteGeometryGeoJson = cargo.RouteGeometryGeoJson;
        entity.RouteCalculatedAt = cargo.RouteCalculatedAt;
        entity.LoadDateTime = cargo.LoadDateTime;
        entity.UnloadDateTime = cargo.UnloadDateTime;
        entity.WeightTons = cargo.WeightTons;
        entity.VolumeM3 = cargo.VolumeM3;
        entity.UseAutomaticCalculation = cargo.UseAutomaticCalculation;
        entity.WeightPerPackageKg = cargo.WeightPerPackageKg;
        entity.BodyTypeRequired = cargo.BodyTypeRequired;
        entity.LoadingType = cargo.LoadingType;
        entity.LengthCm = cargo.LengthCm;
        entity.WidthCm = cargo.WidthCm;
        entity.HeightCm = cargo.HeightCm;
        entity.PalletsCount = cargo.PalletsCount;
        entity.PackagingType = cargo.PackagingType;
        entity.RequiresCMR = cargo.RequiresCMR;
        entity.RequiresTIR = cargo.RequiresTIR;
        entity.IsADR = cargo.IsADR;
        entity.RequiresTwoDrivers = cargo.RequiresTwoDrivers;
        entity.PaymentType = cargo.PaymentType;
        entity.AllowBargaining = cargo.AllowBargaining;
        entity.PrepaymentPercent = cargo.PrepaymentPercent;
        entity.StartingPrice = cargo.StartingPrice;
        entity.BiddingEnabled = cargo.BiddingEnabled;
        entity.MinBidStep = cargo.MinBidStep;
        entity.AcceptedBidId = cargo.AcceptedBidId;
        entity.BiddingClosedAt = cargo.BiddingClosedAt;
        entity.Status = cargo.Status;
        entity.Visibility = cargo.Visibility;
        entity.PublishedAt = cargo.PublishedAt;
        entity.ModeratedAt = cargo.ModeratedAt;
        entity.ModeratedBy = cargo.ModeratedBy;
        entity.RejectionReason = cargo.RejectionReason;
        entity.BoostToTop = cargo.BoostToTop;
        entity.BoostedUntil = cargo.BoostedUntil;
        entity.IsTemplate = cargo.IsTemplate;
        entity.TemplateName = cargo.TemplateName;
        entity.SourceListingId = cargo.SourceListingId;
        entity.Notes = cargo.Notes;
    }

    private static IOrderedQueryable<CargoEntity> ApplySorting(IQueryable<CargoEntity> query, CargoSearchCriteria criteria)
    {
        var now = DateTime.UtcNow;
        var boostedQuery = query.OrderByDescending(c => c.BoostToTop && (!c.BoostedUntil.HasValue || c.BoostedUntil > now));
        var descending = !string.Equals(criteria.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);

        return criteria.SortBy?.Trim().ToLowerInvariant() switch
        {
            "price" or "startingprice" => descending
                ? boostedQuery.ThenByDescending(c => c.StartingPrice)
                : boostedQuery.ThenBy(c => c.StartingPrice),
            "loaddate" or "loaddatetime" => descending
                ? boostedQuery.ThenByDescending(c => c.LoadDateTime)
                : boostedQuery.ThenBy(c => c.LoadDateTime),
            "weight" or "weighttons" => descending
                ? boostedQuery.ThenByDescending(c => c.WeightTons)
                : boostedQuery.ThenBy(c => c.WeightTons),
            "volume" or "volumem3" => descending
                ? boostedQuery.ThenByDescending(c => c.VolumeM3)
                : boostedQuery.ThenBy(c => c.VolumeM3),
            "createdat" => descending
                ? boostedQuery.ThenByDescending(c => c.CreatedAt)
                : boostedQuery.ThenBy(c => c.CreatedAt),
            _ => descending
                ? boostedQuery.ThenByDescending(c => c.PublishedAt ?? c.CreatedAt)
                : boostedQuery.ThenBy(c => c.PublishedAt ?? c.CreatedAt)
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
