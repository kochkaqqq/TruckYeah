namespace RouteService.Infrastructure.Options;

public class OpenRouteServiceOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string DirectionsBaseUrl { get; set; } = "https://api.heigit.org/openrouteservice/v2/directions";
    public string GeocoderBaseUrl { get; set; } = "https://api.heigit.org/pelias/v1";
    public string Profile { get; set; } = "driving-hgv";
}
