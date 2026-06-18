using Microsoft.EntityFrameworkCore;
using RouteService.Domain.Models;
using RouteService.Infrastructure.Interfaces;

namespace RouteService.Infrastructure.Implementation;

public class RouteCalculationsRepository : IRouteCalculationsRepository
{
    private readonly RouteServiceDbContext _dbContext;

    public RouteCalculationsRepository(RouteServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RouteCalculation?> GetActiveByHash(string requestHash, DateTime now)
    {
        return await _dbContext.RouteCalculations
            .Include(r => r.ResolvedPoints)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.RequestHash == requestHash && r.ExpiresAt > now);
    }

    public async Task<Guid> Upsert(RouteCalculation calculation)
    {
        var existing = await _dbContext.RouteCalculations
            .Include(r => r.ResolvedPoints)
            .FirstOrDefaultAsync(r => r.RequestHash == calculation.RequestHash);

        if (existing is null)
        {
            await _dbContext.RouteCalculations.AddAsync(calculation);
            await _dbContext.SaveChangesAsync();
            return calculation.Id;
        }

        _dbContext.ResolvedRoutePoints.RemoveRange(existing.ResolvedPoints);
        existing.Provider = calculation.Provider;
        existing.DistanceKm = calculation.DistanceKm;
        existing.DurationMinutes = calculation.DurationMinutes;
        existing.FuelConsumptionLiters = calculation.FuelConsumptionLiters;
        existing.TollRoadsStatus = calculation.TollRoadsStatus;
        existing.Geometry = calculation.Geometry;
        existing.CreatedAt = calculation.CreatedAt;
        existing.ExpiresAt = calculation.ExpiresAt;
        existing.ResolvedPoints = calculation.ResolvedPoints
            .Select(point => new ResolvedRoutePoint
            {
                Id = point.Id,
                RouteCalculationId = existing.Id,
                Address = point.Address,
                Lat = point.Lat,
                Lon = point.Lon,
                Order = point.Order
            })
            .ToList();

        await _dbContext.SaveChangesAsync();
        return existing.Id;
    }
}
