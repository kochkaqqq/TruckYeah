using RouteService.Application.Interfaces;
using RouteService.Domain.Exceptions;
using RouteService.WebApi.Contracts;
using RouteService.WebApi.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace RouteService.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class RoutesController : ControllerBase
{
    private readonly IRouteCalculationService _routeCalculationService;

    public RoutesController(IRouteCalculationService routeCalculationService)
    {
        _routeCalculationService = routeCalculationService;
    }

    [HttpPost("calculate")]
    public async Task<ActionResult<RouteCalculationResponse>> Calculate([FromBody] CalculateRouteRequest? request, CancellationToken cancellationToken)
    {
        try
        {
            if (request?.Points is null)
            {
                return BadRequest(new ErrorResponse { Message = "Route points are required." });
            }

            var result = await _routeCalculationService.CalculateAsync(
                request.Points.Select(p => p.MapToModel()),
                request.FuelConsumptionLitersPer100Km,
                request.Vehicle?.MapToModel(),
                request.AvoidTollRoads,
                request.AvoidFerries,
                cancellationToken);

            return Ok(result.MapToResponse());
        }
        catch (Exception ex)
        {
            return ToActionResult(ex);
        }
    }

    private ActionResult ToActionResult(Exception ex)
    {
        return ex switch
        {
            ArgumentException => BadRequest(new ErrorResponse { Message = ex.Message }),
            RouteProviderUnavailableException => StatusCode(StatusCodes.Status503ServiceUnavailable, new ErrorResponse { Message = ex.Message }),
            OperationCanceledException => StatusCode(StatusCodes.Status503ServiceUnavailable, new ErrorResponse { Message = "Route provider request timed out." }),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Message = ex.Message })
        };
    }
}
