using ListingService.Domain.Models;
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

    public async Task<Guid> Update(Guid id, Truck truck)
    {
        await _dbContext.Trucks
            .Where(t => t.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(t => t.Title, truck.Title)
                .SetProperty(t => t.Description, truck.Description)
                .SetProperty(t => t.RouteFrom, truck.RouteFrom)
                .SetProperty(t => t.RouteTo, truck.RouteTo)
                .SetProperty(t => t.CapacityTons, truck.CapacityTons)
                .SetProperty(t => t.VolumeM3, truck.VolumeM3)
                .SetProperty(t => t.BodyType, truck.BodyType)
                .SetProperty(t => t.LoadingType, truck.LoadingType)
                .SetProperty(t => t.AvailableFrom, truck.AvailableFrom)
                .SetProperty(t => t.Price, truck.Price)
                .SetProperty(t => t.PaymentType, truck.PaymentType)
                .SetProperty(t => t.AllowBargaining, truck.AllowBargaining)
                .SetProperty(t => t.PrepaymentPercent, truck.PrepaymentPercent));
        
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