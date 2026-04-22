using ListingService.Domain.Models;
using ListingService.Infrastructure.Interfaces;
using ListingService.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace ListingService.Infrastructure.Implementation;

public class CargoRepository : ICargoRepository
{
    private readonly ListingServiceDbContext _dbContext;
    
    public CargoRepository(ListingServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<Cargo>> Get()
    {
        var cargoEntities = await _dbContext.Cargos
            .AsNoTracking()
            .ToListAsync();

        var cargos = cargoEntities.Select(cargoEntity => cargoEntity.MapToModel());
        
        return cargos.ToList();
    }

    public async Task<Guid> Create(Cargo cargo)
    {
        var cargoEntity = cargo.MapToEntity();
        await _dbContext.Cargos.AddAsync(cargoEntity);
        await _dbContext.SaveChangesAsync();
        return cargoEntity.Id;
    }

    public async Task<Guid> Update(Guid id, string title, string description, double? weightKg, double? volumeM3, double? lengthCm,
        double? widthCm, double? heightCm, string cargoType, string? routeFrom, string? routeTo, double? distanceKm,
        DateTime loadDate, decimal? price)
    {
        await _dbContext.Cargos
            .Where(c => c.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.Title, title)
                .SetProperty(c => c.Description, description)
                .SetProperty(c => c.WeightKg, weightKg)
                .SetProperty(c => c.VolumeM3, volumeM3)
                .SetProperty(c => c.LengthCm, lengthCm)
                .SetProperty(c => c.WidthCm, widthCm)
                .SetProperty(c => c.HeightCm, heightCm)
                .SetProperty(c => c.CargoType, cargoType)
                .SetProperty(c => c.RouteFrom, routeFrom)
                .SetProperty(c => c.RouteTo, routeTo)
                .SetProperty(c => c.DistanceKm, distanceKm)
                .SetProperty(c => c.LoadDate, loadDate)
                .SetProperty(c => c.Price, price));
        
        return id;
    }

    public async Task<Guid> Delete(Guid id)
    {
        await _dbContext.Cargos
            .Where(c => c.Id == id)
            .ExecuteDeleteAsync();
        
        return id;
    }
}