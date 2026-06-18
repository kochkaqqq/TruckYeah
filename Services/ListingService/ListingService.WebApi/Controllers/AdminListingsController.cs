using System.Security.Claims;
using ListingService.Application.Interfaces;
using ListingService.WebApi.Contracts.Moderation;
using ListingService.WebApi.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListingService.WebApi.Controllers;

[ApiController]
[Authorize(Roles = "Moderator")]
[Route("api/admin")]
public sealed class AdminListingsController(
    ICargosService cargosService,
    ITrucksService trucksService) : ControllerBase
{
    [HttpGet("cargos")]
    public async Task<IActionResult> GetCargos() =>
        Ok((await cargosService.GetAllForModerationAsync()).Select(c => c.MapToResponse()));

    [HttpGet("trucks")]
    public async Task<IActionResult> GetTrucks() =>
        Ok((await trucksService.GetAllForModerationAsync()).Select(t => t.MapToResponse()));

    [HttpPost("cargos/{id:guid}/approve")]
    public Task<IActionResult> ApproveCargo(Guid id) =>
        ExecuteAsync(() => cargosService.ApproveAsync(id, GetModeratorId()));

    [HttpPost("cargos/{id:guid}/reject")]
    public Task<IActionResult> RejectCargo(Guid id, RejectListingRequest request) =>
        ExecuteAsync(() => cargosService.RejectAsync(id, GetModeratorId(), request.Reason));

    [HttpPost("trucks/{id:guid}/approve")]
    public Task<IActionResult> ApproveTruck(Guid id) =>
        ExecuteAsync(() => trucksService.ApproveAsync(id, GetModeratorId()));

    [HttpPost("trucks/{id:guid}/reject")]
    public Task<IActionResult> RejectTruck(Guid id, RejectListingRequest request) =>
        ExecuteAsync(() => trucksService.RejectAsync(id, GetModeratorId(), request.Reason));

    private Guid GetModeratorId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var id)
            ? id
            : throw new UnauthorizedAccessException("User id claim is missing.");
    }

    private async Task<IActionResult> ExecuteAsync(Func<Task<Guid>> action)
    {
        try
        {
            return Ok(await action());
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
