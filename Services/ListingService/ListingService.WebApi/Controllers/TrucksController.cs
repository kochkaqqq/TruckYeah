using System.Security.Claims;
using ListingService.Application.Interfaces;
using ListingService.WebApi.Contracts.Truck;
using ListingService.WebApi.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListingService.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TrucksController : ControllerBase
{
    private readonly ITrucksService _trucksService;

    public TrucksController(ITrucksService trucksService)
    {
        _trucksService = trucksService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TruckResponse>>> SearchTrucks([FromQuery] TruckSearchQuery query)
    {
        var trucks = await _trucksService.SearchPublishedTrucksAsync(query.MapToCriteria());
        return Ok(trucks.Select(t => t.MapToResponse()));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TruckResponse>> GetTruck(Guid id)
    {
        try
        {
            var truck = await _trucksService.GetTruckByIdAsync(id);
            return Ok(truck.MapToResponse());
        }
        catch (Exception ex)
        {
            return ToActionResult(ex);
        }
    }

    [Authorize]
    [HttpGet("my")]
    public async Task<ActionResult<List<TruckResponse>>> GetMyTrucks()
    {
        try
        {
            var trucks = await _trucksService.GetMyTrucksAsync(GetCurrentUserId());
            return Ok(trucks.Select(t => t.MapToResponse()));
        }
        catch (Exception ex)
        {
            return ToActionResult(ex);
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateTruck([FromBody] CreateTruckRequest request)
    {
        try
        {
            var truck = request.MapToModel(GetCurrentUserId());
            var truckId = await _trucksService.CreateTruckAsync(truck);
            return CreatedAtAction(nameof(GetTruck), new { id = truckId }, truckId);
        }
        catch (Exception ex)
        {
            return ToActionResult(ex);
        }
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Guid>> UpdateTruck(Guid id, [FromBody] UpdateTruckRequest request)
    {
        try
        {
            var truck = request.MapToModel(GetCurrentUserId());
            return Ok(await _trucksService.UpdateTruckAsync(id, GetCurrentUserId(), truck));
        }
        catch (Exception ex)
        {
            return ToActionResult(ex);
        }
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<Guid>> DeleteTruck(Guid id)
    {
        try
        {
            return Ok(await _trucksService.DeleteTruckAsync(id, GetCurrentUserId()));
        }
        catch (Exception ex)
        {
            return ToActionResult(ex);
        }
    }

    [Authorize]
    [HttpPost("{id:guid}/publish")]
    public async Task<ActionResult<Guid>> PublishTruck(Guid id)
    {
        try
        {
            return Ok(await _trucksService.PublishTruckAsync(id, GetCurrentUserId()));
        }
        catch (Exception ex)
        {
            return ToActionResult(ex);
        }
    }

    [Authorize]
    [HttpPost("{id:guid}/archive")]
    public async Task<ActionResult<Guid>> ArchiveTruck(Guid id)
    {
        try
        {
            return Ok(await _trucksService.ArchiveTruckAsync(id, GetCurrentUserId()));
        }
        catch (Exception ex)
        {
            return ToActionResult(ex);
        }
    }

    [Authorize]
    [HttpPost("{id:guid}/copy")]
    public async Task<ActionResult<Guid>> CopyTruck(Guid id)
    {
        try
        {
            return Ok(await _trucksService.CopyTruckAsync(id, GetCurrentUserId()));
        }
        catch (Exception ex)
        {
            return ToActionResult(ex);
        }
    }

    private Guid GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var userId)
            ? userId
            : throw new UnauthorizedAccessException("User id claim is missing.");
    }

    private ActionResult ToActionResult(Exception ex)
    {
        return ex switch
        {
            ArgumentException => BadRequest(ex.Message),
            InvalidOperationException => BadRequest(ex.Message),
            KeyNotFoundException => NotFound(ex.Message),
            UnauthorizedAccessException => Forbid(),
            _ => Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal server error")
        };
    }
}
