using ListingService.Application.Interfaces;
using ListingService.WebApi.Contracts.Cargo;
using ListingService.WebApi.Contracts.Truck;
using ListingService.WebApi.Mappers;
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
    public async Task<ActionResult<List<TruckResponse>>> GetTrucks()
    {
        var trucks = await _trucksService.GetTrucksAsync();

        var response = trucks
            .Select(c => c.MapToResponse());
        
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateTruck([FromBody] TruckRequest request)
    {
        var truck = request.MapToModel();
        var truckId = await _trucksService.CreateTruckAsync(truck);
        return Ok(truckId);
    }
    
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Guid>> UpdateTruck(Guid id, [FromBody] TruckRequest request)
    {
        var truck = request.MapToModel();
        var truckId = await _trucksService.UpdateTruckAsync(id, truck);
        
        return Ok(truckId);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<Guid>> DeleteTruck(Guid id)
    {
        return Ok(await _trucksService.DeleteTruckAsync(id));
    }
}