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
        ValidateCargo(cargo);

        var existing = await GetExistingCargo(id);
        EnsureOwner(existing.UserId, userId);

        cargo.Id = existing.Id;
        cargo.UserId = existing.UserId;
        cargo.CreatedAt = existing.CreatedAt;
        cargo.Status = existing.Status;
        cargo.PublishedAt = existing.PublishedAt;
        cargo.SourceListingId = existing.SourceListingId;
        cargo.BoostedUntil = existing.BoostedUntil;
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

        cargo.Status = ListingStatus.Published;
        cargo.PublishedAt = DateTime.UtcNow;
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

        if (cargo.UserId == carrierUserId)
        {
            throw new InvalidOperationException("Cargo owner cannot bid on their own cargo.");
        }

        var bid = new CargoBid
        {
            Id = Guid.NewGuid(),
            CargoId = cargoId,
            CarrierUserId = carrierUserId,
            Price = price,
            CreatedAt = DateTime.UtcNow
        };

        return await _cargosRepository.CreateBid(bid);
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
            RoutePoints = source.RoutePoints
                .OrderBy(p => p.Order)
                .Select(p => new RoutePoint
                {
                    Id = Guid.NewGuid(),
                    CargoId = copyId,
                    Address = p.Address,
                    ScheduledTime = p.ScheduledTime,
                    Order = p.Order
                })
                .ToList(),
            LoadDateTime = source.LoadDateTime,
            UnloadDateTime = source.UnloadDateTime,
            WeightTons = source.WeightTons,
            VolumeM3 = source.VolumeM3,
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
        if (cargo.PalletsCount < 0)
            throw new ArgumentException("Pallet count cannot be negative.");
        if (cargo.PrepaymentPercent is < 0 or > 100)
            throw new ArgumentException("Prepayment percent must be from 0 to 100.");
        if (cargo.StartingPrice is <= 0)
            throw new ArgumentException("Starting price must be greater than zero.");
        if (cargo.BiddingEnabled && (!cargo.MinBidStep.HasValue || cargo.MinBidStep is <= 0))
            throw new ArgumentException("Minimum bid step must be greater than zero when bidding is enabled.");
    }

    private static void AssignRoutePointIds(Cargo cargo)
    {
        foreach (var point in cargo.RoutePoints)
        {
            point.Id = point.Id == Guid.Empty ? Guid.NewGuid() : point.Id;
            point.CargoId = cargo.Id;
        }
    }

    private static void EnsureOwner(Guid ownerUserId, Guid currentUserId)
    {
        if (ownerUserId != currentUserId)
        {
            throw new UnauthorizedAccessException("Only listing owner can perform this action.");
        }
    }
}
