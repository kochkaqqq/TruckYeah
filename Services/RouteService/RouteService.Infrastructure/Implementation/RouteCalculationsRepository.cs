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

    public async Task<Guid> Create(RouteCalculation calculation)
    {
        await _dbContext.RouteCalculations.AddAsync(calculation);
        await _dbContext.SaveChangesAsync();
        return calculation.Id;
    }
}
