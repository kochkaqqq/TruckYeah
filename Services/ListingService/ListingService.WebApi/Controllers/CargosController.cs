using System.Security.Claims;
using ListingService.Application.Interfaces;
using ListingService.WebApi.Contracts.Cargo;
using ListingService.WebApi.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListingService.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CargosController : ControllerBase
{
    private readonly ICargosService _cargosService;

    public CargosController(ICargosService cargosService)
    {
        _cargosService = cargosService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CargoResponse>>> SearchCargos([FromQuery] CargoSearchQuery query)
    {
        var cargos = await _cargosService.SearchPublishedCargosAsync(query.MapToCriteria());
        return Ok(cargos.Select(c => c.MapToResponse()));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CargoResponse>> GetCargo(Guid id)
    {
        try
        {
            var cargo = await _cargosService.GetCargoByIdAsync(id);
            return Ok(cargo.MapToResponse());
        }
        catch (Exception ex)
        {
            return ToActionResult(ex);
        }
    }

    [Authorize]
    [HttpGet("my")]
    public async Task<ActionResult<List<CargoResponse>>> GetMyCargos()
    {
        try
        {
            var cargos = await _cargosService.GetMyCargosAsync(GetCurrentUserId());
            return Ok(cargos.Select(c => c.MapToResponse()));
        }
        catch (Exception ex)
        {
            return ToActionResult(ex);
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateCargo([FromBody] CreateCargoRequest request)
    {
        try
        {
            var cargo = request.MapToModel(GetCurrentUserId());
            var cargoId = await _cargosService.CreateCargoAsync(cargo);
            return CreatedAtAction(nameof(GetCargo), new { id = cargoId }, cargoId);
        }
        catch (Exception ex)
        {
            return ToActionResult(ex);
        }
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Guid>> UpdateCargo(Guid id, [FromBody] UpdateCargoRequest request)
    {
        try
        {
            var cargo = request.MapToModel(GetCurrentUserId());
            return Ok(await _cargosService.UpdateCargoAsync(id, GetCurrentUserId(), cargo));
        }
        catch (Exception ex)
        {
            return ToActionResult(ex);
        }
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<Guid>> DeleteCargo(Guid id)
    {
        try
        {
            return Ok(await _cargosService.DeleteCargoAsync(id, GetCurrentUserId()));
        }
        catch (Exception ex)
        {
            return ToActionResult(ex);
        }
    }

    [Authorize]
    [HttpPost("{id:guid}/publish")]
    public async Task<ActionResult<Guid>> PublishCargo(Guid id)
    {
        try
        {
            return Ok(await _cargosService.PublishCargoAsync(id, GetCurrentUserId()));
        }
        catch (Exception ex)
        {
            return ToActionResult(ex);
        }
    }

    [Authorize]
    [HttpPost("{id:guid}/archive")]
    public async Task<ActionResult<Guid>> ArchiveCargo(Guid id)
    {
        try
        {
            return Ok(await _cargosService.ArchiveCargoAsync(id, GetCurrentUserId()));
        }
        catch (Exception ex)
        {
            return ToActionResult(ex);
        }
    }

    [Authorize]
    [HttpPost("{id:guid}/copy")]
    public async Task<ActionResult<Guid>> CopyCargo(Guid id)
    {
        try
        {
            return Ok(await _cargosService.CopyCargoAsync(id, GetCurrentUserId()));
        }
        catch (Exception ex)
        {
            return ToActionResult(ex);
        }
    }

    [Authorize]
    [HttpPost("{id:guid}/save-template")]
    public async Task<ActionResult<Guid>> SaveCargoTemplate(Guid id, [FromQuery] string? templateName)
    {
        try
        {
            return Ok(await _cargosService.SaveCargoTemplateAsync(id, GetCurrentUserId(), templateName));
        }
        catch (Exception ex)
        {
            return ToActionResult(ex);
        }
    }

    [Authorize]
    [HttpGet("{id:guid}/bids")]
    public async Task<ActionResult<List<CargoBidResponse>>> GetCargoBids(Guid id)
    {
        try
        {
            var bids = await _cargosService.GetCargoBidsAsync(id, GetCurrentUserId());
            return Ok(bids.Select(b => b.MapToResponse()));
        }
        catch (Exception ex)
        {
            return ToActionResult(ex);
        }
    }

    [Authorize]
    [HttpPost("{id:guid}/bids")]
    public async Task<ActionResult<Guid>> CreateCargoBid(Guid id, [FromBody] CreateCargoBidRequest request)
    {
        try
        {
            return Ok(await _cargosService.CreateCargoBidAsync(id, GetCurrentUserId(), request.Price));
        }
        catch (Exception ex)
        {
            return ToActionResult(ex);
        }
    }

    [Authorize]
    [HttpPost("{id:guid}/bids/{bidId:guid}/accept")]
    public async Task<ActionResult<Guid>> AcceptCargoBid(Guid id, Guid bidId)
    {
        try
        {
            return Ok(await _cargosService.AcceptCargoBidAsync(id, bidId, GetCurrentUserId()));
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
            _ => StatusCode(StatusCodes.Status500InternalServerError, ex.Message)
        };
    }
}
