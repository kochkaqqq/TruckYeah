using ListingService.Application.Interfaces;
using ListingService.Domain.Enums;
using ListingService.Domain.Models;
using ListingService.Infrastructure.Interfaces;

namespace ListingService.Application.Services;

public class CargosService : ICargosService
{
    private readonly ICargosRepository _cargosRepository;

    public CargosService(ICargosRepository cargosRepository)
    {
        _cargosRepository = cargosRepository;
    }

    public Task<List<Cargo>> SearchPublishedCargosAsync(CargoSearchCriteria criteria)
    {
        return _cargosRepository.Search(criteria, publishedOnly: true);
    }

    public Task<List<Cargo>> GetMyCargosAsync(Guid userId)
    {
        return _cargosRepository.Search(new CargoSearchCriteria(), publishedOnly: false, userId);
    }

    public async Task<Cargo> GetCargoByIdAsync(Guid id)
    {
        return await GetExistingCargo(id);
    }

    public async Task<Guid> CreateCargoAsync(Cargo cargo)
    {
        ApplyCalculatedCargoFields(cargo);
        ValidateCargo(cargo);
        cargo.Id = Guid.NewGuid();
        cargo.CreatedAt = DateTime.UtcNow;
        cargo.Status = ListingStatus.Draft;
        cargo.PublishedAt = null;
        AssignRoutePointIds(cargo);

        return await _cargosRepository.Create(cargo);
    }

    public async Task<Guid> UpdateCargoAsync(Guid id, Guid userId, Cargo cargo)
    {
        ApplyCalculatedCargoFields(cargo);
        ValidateCargo(cargo);

        var existing = await GetExistingCargo(id);
        EnsureOwner(existing.UserId, userId);

        cargo.Id = existing.Id;
        cargo.UserId = existing.UserId;
        cargo.CreatedAt = existing.CreatedAt;
        cargo.Status = existing.Status;
        cargo.PublishedAt = existing.PublishedAt;
        cargo.ModeratedAt = existing.ModeratedAt;
        cargo.ModeratedBy = existing.ModeratedBy;
        cargo.RejectionReason = existing.RejectionReason;
        cargo.SourceListingId = existing.SourceListingId;
        cargo.BoostedUntil = existing.BoostedUntil;
        cargo.AcceptedBidId = existing.AcceptedBidId;
        cargo.BiddingClosedAt = existing.BiddingClosedAt;
        AssignRoutePointIds(cargo);

        await _cargosRepository.Update(cargo);
        return id;
    }

    public async Task<Guid> DeleteCargoAsync(Guid id, Guid userId)
    {
        var existing = await GetExistingCargo(id);
        EnsureOwner(existing.UserId, userId);
        await _cargosRepository.Delete(id);
        return id;
    }

    public async Task<Guid> PublishCargoAsync(Guid id, Guid userId)
    {
        var cargo = await GetExistingCargo(id);
        EnsureOwner(cargo.UserId, userId);
        ValidateCargo(cargo);

        cargo.Status = ListingStatus.PendingModeration;
        cargo.PublishedAt = null;
        cargo.ModeratedAt = null;
        cargo.ModeratedBy = null;
        cargo.RejectionReason = null;
        await _cargosRepository.Update(cargo);
        return id;
    }

    public async Task<Guid> ArchiveCargoAsync(Guid id, Guid userId)
    {
        var cargo = await GetExistingCargo(id);
        EnsureOwner(cargo.UserId, userId);

        cargo.Status = ListingStatus.Archived;
        await _cargosRepository.Update(cargo);
        return id;
    }

    public async Task<Guid> CopyCargoAsync(Guid id, Guid userId)
    {
        var source = await GetExistingCargo(id);
        EnsureOwner(source.UserId, userId);

        var copy = CopyCargo(source, userId, isTemplate: false, templateName: null);
        return await _cargosRepository.Create(copy);
    }

    public async Task<Guid> SaveCargoTemplateAsync(Guid id, Guid userId, string? templateName)
    {
        var source = await GetExistingCargo(id);
        EnsureOwner(source.UserId, userId);

        var copy = CopyCargo(source, userId, isTemplate: true, templateName);
        return await _cargosRepository.Create(copy);
    }

    public async Task<List<CargoBid>> GetCargoBidsAsync(Guid cargoId, Guid ownerUserId)
    {
        var cargo = await GetExistingCargo(cargoId);
        EnsureOwner(cargo.UserId, ownerUserId);
        return await _cargosRepository.GetBids(cargoId);
    }

    public async Task<Guid> CreateCargoBidAsync(Guid cargoId, Guid carrierUserId, decimal price)
    {
        if (price <= 0)
        {
            throw new ArgumentException("Bid price must be greater than zero.");
        }

        var cargo = await GetExistingCargo(cargoId);

        if (cargo.Status != ListingStatus.Published || !cargo.BiddingEnabled)
        {
            throw new InvalidOperationException("Bids are available only for published cargos with enabled bidding.");
        }

        if (cargo.AcceptedBidId.HasValue)
        {
            throw new InvalidOperationException("Bidding is already closed for this cargo.");
        }

        if (cargo.UserId == carrierUserId)
        {
            throw new InvalidOperationException("Cargo owner cannot bid on their own cargo.");
        }

        var currentBids = await _cargosRepository.GetBids(cargoId);
        EnsureBidMeetsMinimumStep(cargo, currentBids, price);

        var bid = new CargoBid
        {
            Id = Guid.NewGuid(),
            CargoId = cargoId,
            CarrierUserId = carrierUserId,
            Price = price,
            Status = BidStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        return await _cargosRepository.CreateBid(bid);
    }

    public async Task<Guid> AcceptCargoBidAsync(Guid cargoId, Guid bidId, Guid ownerUserId)
    {
        var cargo = await GetExistingCargo(cargoId);
        EnsureOwner(cargo.UserId, ownerUserId);

        if (!cargo.BiddingEnabled)
        {
            throw new InvalidOperationException("Bidding is not enabled for this cargo.");
        }

        if (cargo.AcceptedBidId.HasValue)
        {
            throw new InvalidOperationException("A bid has already been accepted for this cargo.");
        }

        var bids = await _cargosRepository.GetBids(cargoId);
        var acceptedBid = bids.FirstOrDefault(b => b.Id == bidId)
            ?? throw new KeyNotFoundException("Cargo bid was not found.");

        if (acceptedBid.Status != BidStatus.Active)
        {
            throw new InvalidOperationException("Only active bids can be accepted.");
        }

        var acceptedAt = DateTime.UtcNow;
        foreach (var bid in bids.Where(b => b.Status == BidStatus.Active))
        {
            if (bid.Id == bidId)
            {
                bid.Status = BidStatus.Accepted;
                bid.AcceptedAt = acceptedAt;
            }
            else
            {
                bid.Status = BidStatus.Rejected;
            }
        }

        cargo.AcceptedBidId = bidId;
        cargo.BiddingClosedAt = acceptedAt;
        await _cargosRepository.Update(cargo);
        await _cargosRepository.UpdateBids(bids);

        return bidId;
    }

    private async Task<Cargo> GetExistingCargo(Guid id)
    {
        return await _cargosRepository.GetById(id)
            ?? throw new KeyNotFoundException("Cargo was not found.");
    }

    private static Cargo CopyCargo(Cargo source, Guid userId, bool isTemplate, string? templateName)
    {
        var copyId = Guid.NewGuid();
        var copy = new Cargo
        {
            Id = copyId,
            UserId = userId,
            Title = source.Title,
            CargoName = source.CargoName,
            RouteFrom = source.RouteFrom,
            RouteTo = source.RouteTo,
            RouteDistanceKm = source.RouteDistanceKm,
            RouteDurationMinutes = source.RouteDurationMinutes,
            RouteFuelLiters = source.RouteFuelLiters,
            RouteGeometryGeoJson = source.RouteGeometryGeoJson,
            RouteCalculatedAt = source.RouteCalculatedAt,
            RoutePoints = source.RoutePoints
                .OrderBy(p => p.Order)
                .Select(p => new RoutePoint
                {
                    Id = Guid.NewGuid(),
                    CargoId = copyId,
                    Address = p.Address,
                    Lat = p.Lat,
                    Lon = p.Lon,
                    ScheduledTime = p.ScheduledTime,
                    Order = p.Order
                })
                .ToList(),
            LoadDateTime = source.LoadDateTime,
            UnloadDateTime = source.UnloadDateTime,
            WeightTons = source.WeightTons,
            VolumeM3 = source.VolumeM3,
            UseAutomaticCalculation = source.UseAutomaticCalculation,
            WeightPerPackageKg = source.WeightPerPackageKg,
            BodyTypeRequired = source.BodyTypeRequired,
            LoadingType = source.LoadingType,
            LengthCm = source.LengthCm,
            WidthCm = source.WidthCm,
            HeightCm = source.HeightCm,
            PalletsCount = source.PalletsCount,
            PackagingType = source.PackagingType,
            RequiresCMR = source.RequiresCMR,
            RequiresTIR = source.RequiresTIR,
            IsADR = source.IsADR,
            RequiresTwoDrivers = source.RequiresTwoDrivers,
            PaymentType = source.PaymentType,
            AllowBargaining = source.AllowBargaining,
            PrepaymentPercent = source.PrepaymentPercent,
            StartingPrice = source.StartingPrice,
            BiddingEnabled = source.BiddingEnabled,
            MinBidStep = source.MinBidStep,
            AcceptedBidId = null,
            BiddingClosedAt = null,
            Visibility = source.Visibility,
            BoostToTop = false,
            IsTemplate = isTemplate,
            TemplateName = string.IsNullOrWhiteSpace(templateName) ? source.TemplateName ?? source.Title : templateName,
            SourceListingId = source.Id,
            Notes = source.Notes,
            Status = ListingStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };

        return copy;
    }

    private static void ValidateCargo(Cargo cargo)
    {
        if (string.IsNullOrWhiteSpace(cargo.Title))
            throw new ArgumentException("Title is required.");
        if (string.IsNullOrWhiteSpace(cargo.CargoName))
            throw new ArgumentException("Cargo name is required.");
        if (string.IsNullOrWhiteSpace(cargo.RouteFrom))
            throw new ArgumentException("Route from is required.");
        if (string.IsNullOrWhiteSpace(cargo.RouteTo))
            throw new ArgumentException("Route to is required.");
        ValidateRoute(cargo.RoutePoints, cargo.RouteGeometryGeoJson);
        if (cargo.LoadDateTime == default)
            throw new ArgumentException("Load date and time is required.");
        if (cargo.UnloadDateTime == default)
            throw new ArgumentException("Unload date and time is required.");
        if (cargo.WeightTons <= 0)
            throw new ArgumentException("Weight must be greater than zero.");
        if (cargo.VolumeM3 <= 0)
            throw new ArgumentException("Volume must be greater than zero.");
        if (string.IsNullOrWhiteSpace(cargo.BodyTypeRequired))
            throw new ArgumentException("Body type is required.");
        if (cargo.LengthCm is <= 0)
            throw new ArgumentException("Length must be greater than zero.");
        if (cargo.WidthCm is <= 0)
            throw new ArgumentException("Width must be greater than zero.");
        if (cargo.HeightCm is <= 0)
            throw new ArgumentException("Height must be greater than zero.");
        if (cargo.PalletsCount is <= 0)
            throw new ArgumentException("Pallet count must be greater than zero.");
        if (cargo.PalletsCount < 0)
            throw new ArgumentException("Pallet count cannot be negative.");
        if (cargo.PrepaymentPercent is < 0 or > 100)
            throw new ArgumentException("Prepayment percent must be from 0 to 100.");
        if (cargo.StartingPrice is <= 0)
            throw new ArgumentException("Starting price must be greater than zero.");
        if (cargo.BiddingEnabled && !cargo.StartingPrice.HasValue)
            throw new ArgumentException("Starting price is required when bidding is enabled.");
        if (cargo.BiddingEnabled && (!cargo.MinBidStep.HasValue || cargo.MinBidStep is <= 0))
            throw new ArgumentException("Minimum bid step must be greater than zero when bidding is enabled.");
    }

    private static void ApplyCalculatedCargoFields(Cargo cargo)
    {
        if (!cargo.UseAutomaticCalculation)
        {
            return;
        }

        if (cargo.LengthCm is null or <= 0 ||
            cargo.WidthCm is null or <= 0 ||
            cargo.HeightCm is null or <= 0 ||
            cargo.PalletsCount is null or <= 0 ||
            cargo.WeightPerPackageKg is null or <= 0)
        {
            throw new ArgumentException(
                "Dimensions, package count and weight per package are required for automatic calculation.");
        }

        var volumeM3 = cargo.LengthCm.Value * cargo.WidthCm.Value * cargo.HeightCm.Value * cargo.PalletsCount.Value / 1_000_000d;
        var weightTons = cargo.WeightPerPackageKg.Value * cargo.PalletsCount.Value / 1_000d;
        cargo.VolumeM3 = Math.Round(volumeM3, 3, MidpointRounding.AwayFromZero);
        cargo.WeightTons = Math.Round(weightTons, 3, MidpointRounding.AwayFromZero);
    }

    public Task<List<CargoBid>> GetMyBidsAsync(Guid carrierUserId)
    {
        return _cargosRepository.GetBidsByCarrier(carrierUserId);
    }

    public Task<List<Cargo>> GetAllForModerationAsync() => _cargosRepository.GetAll();

    public async Task<Guid> ApproveAsync(Guid id, Guid moderatorId)
    {
        var cargo = await GetExistingCargo(id);
        if (cargo.Status != ListingStatus.PendingModeration)
        {
            throw new InvalidOperationException("Only listings pending moderation can be approved.");
        }

        cargo.Status = ListingStatus.Published;
        cargo.PublishedAt = DateTime.UtcNow;
        cargo.ModeratedAt = cargo.PublishedAt;
        cargo.ModeratedBy = moderatorId;
        cargo.RejectionReason = null;
        await _cargosRepository.Update(cargo);
        return id;
    }

    public async Task<Guid> RejectAsync(Guid id, Guid moderatorId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Rejection reason is required.");
        }

        var cargo = await GetExistingCargo(id);
        if (cargo.Status != ListingStatus.PendingModeration)
        {
            throw new InvalidOperationException("Only listings pending moderation can be rejected.");
        }

        cargo.Status = ListingStatus.Rejected;
        cargo.PublishedAt = null;
        cargo.ModeratedAt = DateTime.UtcNow;
        cargo.ModeratedBy = moderatorId;
        cargo.RejectionReason = reason.Trim();
        await _cargosRepository.Update(cargo);
        return id;
    }

    private static void EnsureBidMeetsMinimumStep(Cargo cargo, IEnumerable<CargoBid> bids, decimal price)
    {
        var activeBids = bids
            .Where(b => b.Status == BidStatus.Active)
            .ToList();

        var currentBestPrice = activeBids.Count > 0
            ? activeBids.Min(b => b.Price)
            : cargo.StartingPrice;

        if (!currentBestPrice.HasValue || !cargo.MinBidStep.HasValue)
        {
            return;
        }

        var maxAllowedPrice = currentBestPrice.Value - cargo.MinBidStep.Value;
        if (price > maxAllowedPrice)
        {
            throw new InvalidOperationException($"Bid price must be at least {cargo.MinBidStep.Value} lower than the current best price.");
        }
    }

    private static void AssignRoutePointIds(Cargo cargo)
    {
        foreach (var point in cargo.RoutePoints)
        {
            point.Id = point.Id == Guid.Empty ? Guid.NewGuid() : point.Id;
            point.CargoId = cargo.Id;
            point.TruckId = null;
        }
    }

    private static void ValidateRoute(ICollection<RoutePoint> points, string? geometry)
    {
        if (points.Count is < 2 or > 10)
            throw new ArgumentException("Route must contain start, finish and no more than eight intermediate points.");
        if (points.Any(point => point.Lat is < -90 or > 90 || point.Lon is < -180 or > 180))
            throw new ArgumentException("Route point coordinates are invalid.");
        if (string.IsNullOrWhiteSpace(geometry))
            throw new ArgumentException("Route must be calculated before saving.");
    }

    private static void EnsureOwner(Guid ownerUserId, Guid currentUserId)
    {
        if (ownerUserId != currentUserId)
        {
            throw new UnauthorizedAccessException("Only listing owner can perform this action.");
        }
    }
}
