using ListingService.Application.Interfaces;
using ListingService.WebApi.Contracts.Cargo;
using ListingService.WebApi.Mappers;
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
    public async Task<ActionResult<List<CargoResponse>>> GetCargos()
    {
        var cargos = await _cargosService.GetAllCargosAsync();

        var response = cargos
            .Select(c => c.MapToResponse());
        
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateCargo([FromBody] CargoRequest request)
    {
        var cargo = request.MapToModel();
        var cargoId = await _cargosService.CreateCargoAsync(cargo);
        return Ok(cargoId);
    }
    
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Guid>> UpdateCargo(Guid id, [FromBody] CargoRequest request)
    {
        var cargoId = await _cargosService.UpdateCargoAsync(id, request.Title, request.Description, 
            request.WeightKg, request.VolumeM3, request.LengthCm, request.WidthCm, request.HeightCm,
            request.CargoType, request.RouteFrom, request.RouteTo, request.DistanceKm, request.LoadDate, request.Price);
        
        return Ok(cargoId);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<Guid>> DeleteCargo(Guid id)
    {
        return Ok(await _cargosService.DeleteCargoAsync(id));
    }
}