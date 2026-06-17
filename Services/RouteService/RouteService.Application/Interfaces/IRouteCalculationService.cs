using RouteService.Domain.Models;

namespace RouteService.Application.Interfaces;

public interface IRouteCalculationService
{
    Task<RouteCalculationResult> CalculateAsync(
        IEnumerable<RoutePointInput> points,
        double? fuelConsumptionLitersPer100Km,
        RouteVehicleOptions? vehicle,
        bool avoidTollRoads,
        bool avoidFerries,
        CancellationToken cancellationToken);
}
