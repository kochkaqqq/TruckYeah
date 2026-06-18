using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RouteService.Application.Services;
using RouteService.Domain.Enums;
using RouteService.Domain.Models;
using RouteService.Infrastructure.Implementation;
using RouteService.Infrastructure.Interfaces;
using RouteService.Infrastructure.Options;
using Xunit;

namespace RouteService.Tests;

public class RouteCalculationServiceTests
{
    [Fact]
    public async Task CalculateAsync_CalculatesFuelConsumption()
    {
        var repository = new FakeRouteCalculationsRepository();
        var provider = new FakeRouteProvider
        {
            Result = new RouteProviderResult
            {
                DistanceKm = 706.4,
                DurationMinutes = 441,
                TollRoadsStatus = TollRoadsStatus.Unknown
            }
        };
        var service = CreateService(repository, provider);

        var result = await service.CalculateAsync(CreateCoordinatePoints(), 35, null, false, false, CancellationToken.None);

        Assert.Equal(247.2, result.FuelConsumptionLiters);
        Assert.Equal(RouteProvider.OpenRouteService, result.Provider);
    }

    [Fact]
    public async Task CalculateAsync_RejectsLessThanTwoPoints()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CalculateAsync(
                [
                    new RoutePointInput { Address = "Moscow", Order = 0 }
                ],
                35,
                null,
                false,
                false,
                CancellationToken.None));
    }

    [Fact]
    public async Task CalculateAsync_RejectsMoreThanTenPoints()
    {
        var service = CreateService();
        var points = Enumerable.Range(0, 11)
            .Select(index => new RoutePointInput
            {
                Lat = 55 + index,
                Lon = 37 + index,
                Order = index
            });

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CalculateAsync(points, 35, null, false, false, CancellationToken.None));
    }

    [Fact]
    public async Task CalculateAsync_NormalizesPointOrder()
    {
        var provider = new FakeRouteProvider();
        var service = CreateService(provider: provider);

        await service.CalculateAsync(
            [
                new RoutePointInput { Lat = 59.9343, Lon = 30.3351, Order = 2 },
                new RoutePointInput { Lat = 55.7558, Lon = 37.6173, Order = 0 },
                new RoutePointInput { Lat = 57.6261, Lon = 39.8845, Order = 1 }
            ],
            35,
            null,
            false,
            false,
            CancellationToken.None);

        Assert.Equal([0, 1, 2], provider.LastRoutePoints.Select(p => p.Order).ToArray());
    }

    [Fact]
    public async Task CalculateAsync_ReturnsCachedCalculationForSameRoute()
    {
        var repository = new FakeRouteCalculationsRepository();
        var provider = new FakeRouteProvider();
        var service = CreateService(repository, provider);

        var first = await service.CalculateAsync(CreateCoordinatePoints(), 35, null, false, false, CancellationToken.None);
        var second = await service.CalculateAsync(CreateCoordinatePoints(), 35, null, false, false, CancellationToken.None);

        Assert.False(first.FromCache);
        Assert.True(second.FromCache);
        Assert.Equal(1, provider.CalculateCalls);
    }

    [Fact]
    public async Task CalculateAsync_CacheSeparatesAvoidOptions()
    {
        var repository = new FakeRouteCalculationsRepository();
        var provider = new FakeRouteProvider();
        var service = CreateService(repository, provider);

        await service.CalculateAsync(CreateCoordinatePoints(), 35, null, false, false, CancellationToken.None);
        await service.CalculateAsync(CreateCoordinatePoints(), 35, null, true, false, CancellationToken.None);

        Assert.Equal(2, provider.CalculateCalls);
    }

    [Theory]
    [InlineData(TollRoadsStatus.Present)]
    [InlineData(TollRoadsStatus.Absent)]
    [InlineData(TollRoadsStatus.Unknown)]
    public async Task CalculateAsync_MapsTollRoadStatus(TollRoadsStatus tollRoadsStatus)
    {
        var provider = new FakeRouteProvider
        {
            Result = new RouteProviderResult
            {
                DistanceKm = 100,
                DurationMinutes = 60,
                TollRoadsStatus = tollRoadsStatus
            }
        };
        var service = CreateService(provider: provider);

        var result = await service.CalculateAsync(CreateCoordinatePoints(), 35, null, false, false, CancellationToken.None);

        Assert.Equal(tollRoadsStatus, result.TollRoadsStatus);
    }

    [Fact]
    public async Task CalculateAsync_PassesVehicleAndAvoidOptionsToProvider()
    {
        var provider = new FakeRouteProvider();
        var service = CreateService(provider: provider);
        var vehicle = new RouteVehicleOptions
        {
            HeightMeters = 4,
            WidthMeters = 2.5,
            LengthMeters = 12,
            WeightTons = 20,
            Hazmat = true
        };

        await service.CalculateAsync(CreateCoordinatePoints(), 35, vehicle, true, true, CancellationToken.None);

        Assert.True(provider.LastAvoidTollRoads);
        Assert.True(provider.LastAvoidFerries);
        Assert.Equal(4, provider.LastVehicle?.HeightMeters);
        Assert.Equal(20, provider.LastVehicle?.WeightTons);
        Assert.True(provider.LastVehicle?.Hazmat);
    }

    [Fact]
    public async Task OpenRouteServiceRouteProvider_SendsHgvGeoJsonRequest()
    {
        var handler = new CapturingHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                """
                {
                  "type": "FeatureCollection",
                  "features": [
                    {
                      "type": "Feature",
                      "properties": {
                        "summary": {
                          "distance": 12345.6,
                          "duration": 678.9
                        }
                      },
                      "geometry": {
                        "type": "LineString",
                        "coordinates": [[14.4213, 50.0875], [17.1077, 48.1486]]
                      }
                    }
                  ]
                }
                """,
                Encoding.UTF8,
                "application/json")
        });
        var provider = CreateOpenRouteServiceProvider(handler);

        var result = await provider.CalculateRouteAsync(
            [
                new ResolvedRoutePoint { Lat = 50.0875, Lon = 14.4213, Order = 0 },
                new ResolvedRoutePoint { Lat = 48.1486, Lon = 17.1077, Order = 1 }
            ],
            new RouteVehicleOptions
            {
                HeightMeters = 4,
                WidthMeters = 2.5,
                LengthMeters = 12,
                WeightTons = 20,
                AxleLoadTons = 8,
                Hazmat = true
            },
            true,
            true,
            CancellationToken.None);

        Assert.Equal("https://api.heigit.org/openrouteservice/v2/directions/driving-hgv/geojson", handler.LastRequestUri);
        Assert.Equal("test-key", handler.LastAuthorizationHeader);
        Assert.Equal(12.3, result.DistanceKm);
        Assert.Equal(12, result.DurationMinutes);
        Assert.NotNull(result.Geometry.GeoJson);

        using var document = JsonDocument.Parse(handler.LastRequestBody);
        var root = document.RootElement;
        Assert.Equal(14.4213, root.GetProperty("coordinates")[0][0].GetDouble());
        Assert.Equal(50.0875, root.GetProperty("coordinates")[0][1].GetDouble());
        Assert.Contains("tollways", root.GetProperty("options").GetProperty("avoid_features").EnumerateArray().Select(x => x.GetString()));
        Assert.Contains("ferries", root.GetProperty("options").GetProperty("avoid_features").EnumerateArray().Select(x => x.GetString()));
        Assert.Equal("hgv", root.GetProperty("options").GetProperty("vehicle_type").GetString());
        var restrictions = root.GetProperty("options").GetProperty("profile_params").GetProperty("restrictions");
        Assert.Equal(4, restrictions.GetProperty("height").GetDouble());
        Assert.Equal(20, restrictions.GetProperty("weight").GetDouble());
        Assert.True(restrictions.GetProperty("hazmat").GetBoolean());
    }

    [Fact]
    public async Task OpenRouteServiceRouteProvider_GeocodesFirstFeature()
    {
        var handler = new CapturingHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                """
                {
                  "type": "FeatureCollection",
                  "features": [
                    {
                      "type": "Feature",
                      "properties": {
                        "label": "Prague, Czechia"
                      },
                      "geometry": {
                        "type": "Point",
                        "coordinates": [14.4213, 50.0875]
                      }
                    }
                  ]
                }
                """,
                Encoding.UTF8,
                "application/json")
        });
        var provider = CreateOpenRouteServiceProvider(handler);

        var result = await provider.GeocodeAsync("Prague", 1, CancellationToken.None);

        Assert.Equal("https://api.heigit.org/pelias/v1/search?text=Prague", handler.LastRequestUri);
        Assert.Equal("test-key", handler.LastAuthorizationHeader);
        Assert.Equal("Prague, Czechia", result.Address);
        Assert.Equal(50.0875, result.Lat);
        Assert.Equal(14.4213, result.Lon);
        Assert.Equal(1, result.Order);
    }

    private static RouteCalculationService CreateService(
        FakeRouteCalculationsRepository? repository = null,
        FakeRouteProvider? provider = null)
    {
        return new RouteCalculationService(
            repository ?? new FakeRouteCalculationsRepository(),
            provider ?? new FakeRouteProvider(),
            Options.Create(new RouteCalculationOptions
            {
                DefaultFuelConsumptionLitersPer100Km = 35,
                CacheTtlHours = 24,
                RequestTimeoutSeconds = 10
            }));
    }

    private static OpenRouteServiceRouteProvider CreateOpenRouteServiceProvider(CapturingHandler handler)
    {
        return new OpenRouteServiceRouteProvider(
            new HttpClient(handler),
            Options.Create(new OpenRouteServiceOptions
            {
                ApiKey = "test-key",
                DirectionsBaseUrl = "https://api.heigit.org/openrouteservice/v2/directions",
                GeocoderBaseUrl = "https://api.heigit.org/pelias/v1",
                Profile = "driving-hgv"
            }));
    }

    private static List<RoutePointInput> CreateCoordinatePoints()
    {
        return
        [
            new RoutePointInput { Lat = 55.7558, Lon = 37.6173, Order = 0 },
            new RoutePointInput { Lat = 59.9343, Lon = 30.3351, Order = 1 }
        ];
    }

    private sealed class FakeRouteCalculationsRepository : IRouteCalculationsRepository
    {
        private readonly Dictionary<string, RouteCalculation> _cache = [];

        public Task<RouteCalculation?> GetActiveByHash(string requestHash, DateTime now)
        {
            return Task.FromResult(
                _cache.TryGetValue(requestHash, out var calculation) && calculation.ExpiresAt > now
                    ? calculation
                    : null);
        }

        public Task<Guid> Upsert(RouteCalculation routeCalculation)
        {
            _cache[routeCalculation.RequestHash] = routeCalculation;
            return Task.FromResult(routeCalculation.Id);
        }
    }

    private sealed class FakeRouteProvider : IRouteProvider
    {
        public RouteProviderResult Result { get; init; } = new()
        {
            DistanceKm = 706.4,
            DurationMinutes = 441,
            TollRoadsStatus = TollRoadsStatus.Unknown
        };

        public int CalculateCalls { get; private set; }
        public IReadOnlyList<ResolvedRoutePoint> LastRoutePoints { get; private set; } = [];
        public RouteVehicleOptions? LastVehicle { get; private set; }
        public bool LastAvoidTollRoads { get; private set; }
        public bool LastAvoidFerries { get; private set; }

        public Task<ResolvedRoutePoint> GeocodeAsync(string address, int order, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ResolvedRoutePoint
            {
                Id = Guid.NewGuid(),
                Address = address,
                Lat = 55.7558 + order,
                Lon = 37.6173 + order,
                Order = order
            });
        }

        public Task<RouteProviderResult> CalculateRouteAsync(
            IReadOnlyList<ResolvedRoutePoint> points,
            RouteVehicleOptions? vehicle,
            bool avoidTollRoads,
            bool avoidFerries,
            CancellationToken cancellationToken)
        {
            CalculateCalls++;
            LastRoutePoints = points.ToList();
            LastVehicle = vehicle;
            LastAvoidTollRoads = avoidTollRoads;
            LastAvoidFerries = avoidFerries;
            return Task.FromResult(Result);
        }
    }

    private sealed class CapturingHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;

        public CapturingHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _responseFactory = responseFactory;
        }

        public string LastRequestBody { get; private set; } = string.Empty;
        public string LastRequestUri { get; private set; } = string.Empty;
        public string? LastAuthorizationHeader { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            LastRequestUri = request.RequestUri?.ToString() ?? string.Empty;
            LastAuthorizationHeader = request.Headers.TryGetValues("Authorization", out var values)
                ? values.SingleOrDefault()
                : null;

            LastRequestBody = request.Content is null
                ? string.Empty
                : await request.Content.ReadAsStringAsync(cancellationToken);

            return _responseFactory(request);
        }
    }
}
