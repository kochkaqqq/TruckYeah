using RouteService.Domain.Models;

namespace RouteService.Infrastructure.Interfaces;

public interface IRouteProvider
{
    Task<ResolvedRoutePoint> GeocodeAsync(string address, int order, CancellationToken cancellationToken);
    Task<RouteProviderResult> CalculateRouteAsync(
        IReadOnlyList<ResolvedRoutePoint> points,
        RouteVehicleOptions? vehicle,
        bool avoidTollRoads,
        bool avoidFerries,
        CancellationToken cancellationToken);
}
