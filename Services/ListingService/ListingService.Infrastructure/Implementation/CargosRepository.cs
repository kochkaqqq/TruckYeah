using ListingService.Domain.Models;
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

    public async Task<Guid> Update(Guid id, Cargo cargo)
    {
        await _dbContext.Cargos
            .Where(c => c.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.Title, cargo.Title)
                .SetProperty(c => c.CargoName, cargo.CargoName)
                .SetProperty(c => c.RouteFrom, cargo.RouteFrom)
                .SetProperty(c => c.RouteTo, cargo.RouteTo)
                .SetProperty(c => c.RoutePoints, cargo.RoutePoints)
                .SetProperty(c => c.WeightTons, cargo.WeightTons)
                .SetProperty(c => c.VolumeM3, cargo.VolumeM3)
                .SetProperty(c => c.BodyTypeRequired, cargo.BodyTypeRequired)
                .SetProperty(c => c.LoadingType, cargo.LoadingType)
                .SetProperty(c => c.LengthCm, cargo.LengthCm)
                .SetProperty(c => c.WidthCm , cargo.WidthCm)
                .SetProperty(c => c.HeightCm, cargo.HeightCm)
                .SetProperty(c => c.PalletsCount, cargo.PalletsCount)
                .SetProperty(c => c.PackagingType, cargo.PackagingType)
                .SetProperty(c => c.RequiresCMR, cargo.RequiresCMR)
                .SetProperty(c => c.RequiresTIR, cargo.RequiresTIR)
                .SetProperty(c => c.IsADR, cargo.IsADR)
                .SetProperty(c => c.PaymentType, cargo.PaymentType)
                .SetProperty(c => c.AllowBargaining, cargo.AllowBargaining)
                .SetProperty(c => c.PrepaymentPercent, cargo.PrepaymentPercent)
                .SetProperty(c => c.Notes, cargo.Notes));
        
        await UpdateRoutePoints(id, cargo.RoutePoints);
        return id;
    }

    public async Task<Guid> Delete(Guid id)
    {
        await _dbContext.Cargos
            .Where(c => c.Id == id)
            .ExecuteDeleteAsync();
        
        return id;
    }
    
    private async Task UpdateRoutePoints(Guid cargoId, ICollection<RoutePoint> newRoutePoints)
    {
        var oldPoints = _dbContext.RoutePoints
            .Where(rp => rp.CargoId == cargoId);
        await _dbContext.RoutePoints
            .Where(rp => rp.CargoId == cargoId)
            .ExecuteDeleteAsync();
        
        if (newRoutePoints != null && newRoutePoints.Any())
        {
            foreach (var point in newRoutePoints)
            {
                point.CargoId = cargoId;
                await _dbContext.RoutePoints.AddAsync(point);
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}