using ListingService.Domain.Models;
using ListingService.Infrastructure.Interfaces;
using ListingService.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace ListingService.Infrastructure.Implementation;

public class TruckRepository : ITruckRepository
{
    private readonly ListingServiceDbContext _dbContext;

    public TruckRepository(ListingServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<Truck>> Get()
    {
        var truckEntities = await _dbContext.Trucks
            .AsNoTracking()
            .ToListAsync();

        var trucks = truckEntities.Select(t => t.MapToModel());
        
        return trucks.ToList();
    }

    public async Task<Guid> Create(Truck truck)
    {
        var truckEntity = truck.MapToEntity();
        await _dbContext.Trucks.AddAsync(truckEntity);
        await _dbContext.SaveChangesAsync();
        return truckEntity.Id;
    }

    public async Task<Guid> Update(Guid id, string title, string description, string bodyType, double? capacityKg, double? volumeM3,
        double? lengthCm, double? widthCm, double? heightCm, string currentLocation, string routeFrom, string routeTo,
        int? radiusKm, decimal? pricePerKm, string currency)
    {
        await _dbContext.Trucks.Where(t => t.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(t => t.Title, title)
                .SetProperty(t => t.Description, description)
                .SetProperty(t => t.BodyType, bodyType)
                .SetProperty(t => t.CapacityKg, capacityKg)
                .SetProperty(t => t.VolumeM3, volumeM3)
                .SetProperty(t => t.LengthCm, lengthCm)
                .SetProperty(t => t.WidthCm, widthCm)
                .SetProperty(t => t.HeightCm, heightCm)
                .SetProperty(t => t.CurrentLocation, currentLocation)
                .SetProperty(t => t.RouteFrom, routeFrom)
                .SetProperty(t => t.RouteTo, routeTo)
                .SetProperty(t => t.RadiusKm, radiusKm)
                .SetProperty(t => t.PricePerKm, pricePerKm)
                .SetProperty(t => t.Currency, currency));

        return id;
    }

    public async Task<Guid> Delete(Guid id)
    {
        await _dbContext.Trucks
            .Where(t => t.Id == id)
            .ExecuteDeleteAsync();
        return id;
    }
}