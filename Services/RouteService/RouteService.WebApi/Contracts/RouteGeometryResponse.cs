using System.Text.Json;

namespace RouteService.WebApi.Contracts;

public class RouteGeometryResponse
{
    public string? Polyline { get; set; }
    public JsonElement? GeoJson { get; set; }
}
