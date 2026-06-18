using RouteService.Domain.Models;

namespace RouteService.Infrastructure.Interfaces;

public interface IRouteCalculationsRepository
{
    Task<RouteCalculation?> GetActiveByHash(string requestHash, DateTime now);
    Task<Guid> Upsert(RouteCalculation calculation);
}
