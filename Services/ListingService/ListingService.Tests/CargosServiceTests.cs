using ListingService.Application.Services;
using ListingService.Domain.Enums;
using ListingService.Domain.Models;
using ListingService.Infrastructure.Interfaces;

namespace ListingService.Tests;

public class CargosServiceTests
{
    [Fact]
    public async Task CreateCargo_AutomaticMode_RecalculatesWeightAndVolume()
    {
        var repository = new FakeCargosRepository();
        var service = new CargosService(repository);
        var cargo = CreateValidCargo();
        cargo.UseAutomaticCalculation = true;
        cargo.LengthCm = 120;
        cargo.WidthCm = 80;
        cargo.HeightCm = 100;
        cargo.PalletsCount = 4;
        cargo.WeightPerPackageKg = 250;
        cargo.WeightTons = 99;
        cargo.VolumeM3 = 99;

        await service.CreateCargoAsync(cargo);

        Assert.NotNull(repository.Created);
        Assert.Equal(1, repository.Created.WeightTons, 3);
        Assert.Equal(3.84, repository.Created.VolumeM3, 3);
    }

    [Fact]
    public async Task CreateCargo_AutomaticMode_RequiresPackageWeight()
    {
        var service = new CargosService(new FakeCargosRepository());
        var cargo = CreateValidCargo();
        cargo.UseAutomaticCalculation = true;
        cargo.LengthCm = 120;
        cargo.WidthCm = 80;
        cargo.HeightCm = 100;
        cargo.PalletsCount = 4;
        cargo.WeightPerPackageKg = null;

        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateCargoAsync(cargo));

        Assert.Contains("weight per package", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PublishCargo_MovesListingToModerationQueue()
    {
        var cargo = CreateValidCargo();
        cargo.Id = Guid.NewGuid();
        cargo.Status = ListingStatus.Draft;
        var repository = new FakeCargosRepository { Existing = cargo };
        var service = new CargosService(repository);

        await service.PublishCargoAsync(cargo.Id, cargo.UserId);

        Assert.Equal(ListingStatus.PendingModeration, repository.Updated?.Status);
        Assert.Null(repository.Updated?.PublishedAt);
    }

    [Fact]
    public async Task ApproveCargo_PublishesPendingListing()
    {
        var cargo = CreateValidCargo();
        cargo.Id = Guid.NewGuid();
        cargo.Status = ListingStatus.PendingModeration;
        var moderatorId = Guid.NewGuid();
        var repository = new FakeCargosRepository { Existing = cargo };
        var service = new CargosService(repository);

        await service.ApproveAsync(cargo.Id, moderatorId);

        Assert.Equal(ListingStatus.Published, repository.Updated?.Status);
        Assert.Equal(moderatorId, repository.Updated?.ModeratedBy);
        Assert.NotNull(repository.Updated?.PublishedAt);
    }

    [Fact]
    public async Task RejectCargo_RequiresReason()
    {
        var cargo = CreateValidCargo();
        cargo.Id = Guid.NewGuid();
        cargo.Status = ListingStatus.PendingModeration;
        var service = new CargosService(new FakeCargosRepository { Existing = cargo });

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.RejectAsync(cargo.Id, Guid.NewGuid(), " "));
    }

    private static Cargo CreateValidCargo()
    {
        return new Cargo
        {
            UserId = Guid.NewGuid(),
            Title = "Test cargo",
            CargoName = "Equipment",
            RouteFrom = "Moscow",
            RouteTo = "Kazan",
            LoadDateTime = DateTime.UtcNow.AddDays(1),
            UnloadDateTime = DateTime.UtcNow.AddDays(2),
            WeightTons = 1,
            VolumeM3 = 1,
            BodyTypeRequired = "Tent",
            LoadingType = LoadingType.Rear,
            PaymentType = PaymentType.Cash,
            StartingPrice = 1000,
            BiddingEnabled = false,
            Visibility = ListingVisibility.Exchange
            ,
            RouteGeometryGeoJson = """{"type":"LineString","coordinates":[[37.6173,55.7558],[49.1064,55.7961]]}""",
            RoutePoints =
            [
                new RoutePoint { Address = "Moscow", Lat = 55.7558, Lon = 37.6173, Order = 0 },
                new RoutePoint { Address = "Kazan", Lat = 55.7961, Lon = 49.1064, Order = 1 }
            ]
        };
    }

    private sealed class FakeCargosRepository : ICargosRepository
    {
        public Cargo? Created { get; private set; }
        public Cargo? Existing { get; set; }
        public Cargo? Updated { get; private set; }

        public Task<List<Cargo>> Search(CargoSearchCriteria criteria, bool publishedOnly, Guid? userId = null) =>
            Task.FromResult(new List<Cargo>());

        public Task<Cargo?> GetById(Guid id) => Task.FromResult(Existing);

        public Task<Guid> Create(Cargo cargo)
        {
            Created = cargo;
            return Task.FromResult(cargo.Id);
        }

        public Task Update(Cargo cargo)
        {
            Updated = cargo;
            return Task.CompletedTask;
        }
        public Task Delete(Guid id) => Task.CompletedTask;
        public Task<List<CargoBid>> GetBids(Guid cargoId) => Task.FromResult(new List<CargoBid>());
        public Task<List<CargoBid>> GetBidsByCarrier(Guid carrierUserId) => Task.FromResult(new List<CargoBid>());
        public Task<Guid> CreateBid(CargoBid bid) => Task.FromResult(bid.Id);
        public Task UpdateBids(IEnumerable<CargoBid> bids) => Task.CompletedTask;
        public Task<List<Cargo>> GetAll() => Task.FromResult(new List<Cargo>());
    }
}
