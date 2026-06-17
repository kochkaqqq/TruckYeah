using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RouteService.Domain.Enums;
using RouteService.Domain.Exceptions;
using RouteService.Domain.Models;
using RouteService.Infrastructure.Interfaces;
using RouteService.Infrastructure.Options;

namespace RouteService.Infrastructure.Implementation;

public class OpenRouteServiceRouteProvider : IRouteProvider
{
    private readonly HttpClient _httpClient;
    private readonly OpenRouteServiceOptions _options;

    public OpenRouteServiceRouteProvider(HttpClient httpClient, IOptions<OpenRouteServiceOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<ResolvedRoutePoint> GeocodeAsync(string address, int order, CancellationToken cancellationToken)
    {
        EnsureConfigured();

        using var request = new HttpRequestMessage(HttpMethod.Get, BuildGeocoderUrl(address));
        AddAuthorization(request);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw CreateProviderException("geocoder", response.StatusCode, body);
        }

        using var document = JsonDocument.Parse(body);
        if (!TryReadFirstGeocoderFeature(document.RootElement, out var resolved))
        {
            throw new ArgumentException($"Address was not found: {address}");
        }

        resolved.Id = Guid.NewGuid();
        resolved.RouteCalculationId = Guid.Empty;
        resolved.Order = order;
        resolved.Address = string.IsNullOrWhiteSpace(resolved.Address) ? address : resolved.Address;

        return resolved;
    }

    public async Task<RouteProviderResult> CalculateRouteAsync(
        IReadOnlyList<ResolvedRoutePoint> points,
        RouteVehicleOptions? vehicle,
        bool avoidTollRoads,
        bool avoidFerries,
        CancellationToken cancellationToken)
    {
        EnsureConfigured();

        using var request = new HttpRequestMessage(HttpMethod.Post, BuildDirectionsUrl())
        {
            Content = JsonContent.Create(BuildDirectionsPayload(points, vehicle, avoidTollRoads, avoidFerries))
        };
        AddAuthorization(request);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent || string.IsNullOrWhiteSpace(body))
        {
            throw new ArgumentException("Route was not found for the provided points.");
        }

        if (!response.IsSuccessStatusCode)
        {
            throw CreateProviderException("directions", response.StatusCode, body);
        }

        using var document = JsonDocument.Parse(body);
        var feature = GetFirstRouteFeature(document.RootElement);
        var summary = GetSummary(feature);

        var distanceMeters = ReadRequiredNumber(summary, "distance", "OpenRouteService response does not contain distance.");
        var durationSeconds = ReadRequiredNumber(summary, "duration", "OpenRouteService response does not contain duration.");
        var geometry = ReadGeometry(feature);

        return new RouteProviderResult
        {
            DistanceKm = Math.Round(distanceMeters / 1000d, 1, MidpointRounding.AwayFromZero),
            DurationMinutes = Math.Max(1, (int)Math.Ceiling(durationSeconds / 60d)),
            TollRoadsStatus = TollRoadsStatus.Unknown,
            Geometry = new RouteGeometry
            {
                GeoJson = geometry.GetRawText()
            },
            Warnings = ["OpenRouteService response does not contain reliable toll road presence data."]
        };
    }

    private string BuildGeocoderUrl(string address)
    {
        return $"{TrimTrailingSlash(_options.GeocoderBaseUrl)}/search?text={Uri.EscapeDataString(address)}";
    }

    private string BuildDirectionsUrl()
    {
        var profile = string.IsNullOrWhiteSpace(_options.Profile) ? "driving-hgv" : _options.Profile.Trim();
        return $"{TrimTrailingSlash(_options.DirectionsBaseUrl)}/{Uri.EscapeDataString(profile)}/geojson";
    }

    private static object BuildDirectionsPayload(
        IReadOnlyList<ResolvedRoutePoint> points,
        RouteVehicleOptions? vehicle,
        bool avoidTollRoads,
        bool avoidFerries)
    {
        var payload = new Dictionary<string, object?>
        {
            ["coordinates"] = points
                .OrderBy(p => p.Order)
                .Select(p => new[] { p.Lon, p.Lat })
                .ToArray(),
            ["instructions"] = false,
            ["geometry"] = true
        };

        var routingOptions = BuildRoutingOptions(vehicle, avoidTollRoads, avoidFerries);
        if (routingOptions.Count > 0)
        {
            payload["options"] = routingOptions;
        }

        return payload;
    }

    private static Dictionary<string, object> BuildRoutingOptions(
        RouteVehicleOptions? vehicle,
        bool avoidTollRoads,
        bool avoidFerries)
    {
        var options = new Dictionary<string, object>();
        var avoidFeatures = new List<string>();

        if (avoidTollRoads)
        {
            avoidFeatures.Add("tollways");
        }

        if (avoidFerries)
        {
            avoidFeatures.Add("ferries");
        }

        if (avoidFeatures.Count > 0)
        {
            options["avoid_features"] = avoidFeatures;
        }

        if (vehicle is null)
        {
            return options;
        }

        options["vehicle_type"] = "hgv";
        var restrictions = new Dictionary<string, object>();

        AddRestriction(restrictions, "height", vehicle.HeightMeters);
        AddRestriction(restrictions, "width", vehicle.WidthMeters);
        AddRestriction(restrictions, "length", vehicle.LengthMeters);
        AddRestriction(restrictions, "weight", vehicle.WeightTons);
        AddRestriction(restrictions, "axleload", vehicle.AxleLoadTons);

        if (vehicle.Hazmat)
        {
            restrictions["hazmat"] = true;
        }

        if (restrictions.Count > 0)
        {
            options["profile_params"] = new Dictionary<string, object>
            {
                ["restrictions"] = restrictions
            };
        }

        return options;
    }

    private static void AddRestriction(IDictionary<string, object> restrictions, string name, double? value)
    {
        if (value.HasValue)
        {
            restrictions[name] = value.Value;
        }
    }

    private void AddAuthorization(HttpRequestMessage request)
    {
        request.Headers.TryAddWithoutValidation("Authorization", _options.ApiKey);
    }

    private void EnsureConfigured()
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new RouteProviderUnavailableException("OpenRouteService API key is not configured.");
        }
    }

    private static Exception CreateProviderException(string apiName, HttpStatusCode statusCode, string body)
    {
        var providerMessage = TryReadProviderErrorMessage(body);
        var details = string.IsNullOrWhiteSpace(providerMessage)
            ? string.Empty
            : $" Provider message: {providerMessage}";

        if (statusCode == HttpStatusCode.BadRequest)
        {
            return new ArgumentException($"OpenRouteService {apiName} request was rejected.{details}");
        }

        if (statusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
        {
            return new RouteProviderUnavailableException(
                $"OpenRouteService {apiName} request failed with {(int)statusCode} {statusCode}.{details} Check that the API key has access to this API.");
        }

        if ((int)statusCode == 429)
        {
            return new RouteProviderUnavailableException(
                $"OpenRouteService {apiName} request exceeded API limits.{details}");
        }

        return new RouteProviderUnavailableException(
            $"OpenRouteService {apiName} request failed with {(int)statusCode} {statusCode}.{details}");
    }

    private static bool TryReadFirstGeocoderFeature(JsonElement root, out ResolvedRoutePoint point)
    {
        point = new ResolvedRoutePoint();

        if (!root.TryGetProperty("features", out var features) ||
            features.ValueKind != JsonValueKind.Array ||
            features.GetArrayLength() == 0)
        {
            return false;
        }

        var feature = features.EnumerateArray().First();
        if (!feature.TryGetProperty("geometry", out var geometry) ||
            !geometry.TryGetProperty("coordinates", out var coordinates) ||
            coordinates.ValueKind != JsonValueKind.Array ||
            coordinates.GetArrayLength() < 2)
        {
            return false;
        }

        var lon = coordinates[0].GetDouble();
        var lat = coordinates[1].GetDouble();
        var address = ReadGeocoderAddress(feature);

        point = new ResolvedRoutePoint
        {
            Address = address,
            Lat = lat,
            Lon = lon
        };

        return true;
    }

    private static string ReadGeocoderAddress(JsonElement feature)
    {
        if (!feature.TryGetProperty("properties", out var properties))
        {
            return string.Empty;
        }

        foreach (var propertyName in new[] { "label", "name", "locality", "region" })
        {
            if (properties.TryGetProperty(propertyName, out var property) &&
                property.ValueKind == JsonValueKind.String)
            {
                return property.GetString() ?? string.Empty;
            }
        }

        return string.Empty;
    }

    private static JsonElement GetFirstRouteFeature(JsonElement root)
    {
        if (!root.TryGetProperty("features", out var features) ||
            features.ValueKind != JsonValueKind.Array ||
            features.GetArrayLength() == 0)
        {
            throw new ArgumentException("Route was not found for the provided points.");
        }

        return features.EnumerateArray().First();
    }

    private static JsonElement GetSummary(JsonElement feature)
    {
        if (!feature.TryGetProperty("properties", out var properties) ||
            !properties.TryGetProperty("summary", out var summary) ||
            summary.ValueKind != JsonValueKind.Object)
        {
            throw new RouteProviderUnavailableException("OpenRouteService response does not contain route summary.");
        }

        return summary;
    }

    private static JsonElement ReadGeometry(JsonElement feature)
    {
        if (!feature.TryGetProperty("geometry", out var geometry) ||
            geometry.ValueKind != JsonValueKind.Object)
        {
            throw new RouteProviderUnavailableException("OpenRouteService response does not contain route geometry.");
        }

        return geometry;
    }

    private static double ReadRequiredNumber(JsonElement element, string propertyName, string errorMessage)
    {
        if (!element.TryGetProperty(propertyName, out var value))
        {
            throw new RouteProviderUnavailableException(errorMessage);
        }

        return value.ValueKind switch
        {
            JsonValueKind.Number when value.TryGetDouble(out var number) => number,
            JsonValueKind.String when double.TryParse(value.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var number) => number,
            _ => throw new RouteProviderUnavailableException(errorMessage)
        };
    }

    private static string? TryReadProviderErrorMessage(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(body);
            return TryFindString(document.RootElement, ["message", "detail", "error"]);
        }
        catch (JsonException)
        {
            return body.Length > 300 ? body[..300] : body;
        }
    }

    private static string? TryFindString(JsonElement element, string[] propertyNames)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in element.EnumerateObject())
            {
                if (propertyNames.Any(name => string.Equals(name, property.Name, StringComparison.OrdinalIgnoreCase)) &&
                    property.Value.ValueKind == JsonValueKind.String)
                {
                    return property.Value.GetString();
                }

                var nested = TryFindString(property.Value, propertyNames);
                if (!string.IsNullOrWhiteSpace(nested))
                {
                    return nested;
                }
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in element.EnumerateArray())
            {
                var nested = TryFindString(item, propertyNames);
                if (!string.IsNullOrWhiteSpace(nested))
                {
                    return nested;
                }
            }
        }

        return null;
    }

    private static string TrimTrailingSlash(string value)
    {
        return value.Trim().TrimEnd('/');
    }
}
