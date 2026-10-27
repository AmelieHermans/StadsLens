using Microsoft.AspNetCore.Mvc;
using StadsLens.Application.Interfaces.Services;
using StadsLens.Contracts.Landmarks;
using StadsLens.Contracts.Routes;

namespace StadsLens.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoutesController : ControllerBase
{
    private readonly IRouteService _routeService;
    private readonly ILandmarkService _landmarkService;

    public RoutesController(IRouteService routeService, ILandmarkService landmarkService)
    {
        _routeService = routeService;
        _landmarkService = landmarkService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RouteResponse>>> GetAll()
    {
        return Ok(await _routeService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RouteResponse>> GetById(int id)
    {
        var route = await _routeService.GetByIdAsync(id);
        return route is null ? NotFound() : Ok(route);
    }

    [HttpGet("{id:int}/landmarks")]
    public async Task<ActionResult<IEnumerable<LandmarkResponse>>> GetLandmarks(int id)
    {
        return Ok(await _landmarkService.GetByRouteIdAsync(id));
    }

    [HttpPost("plan")]
    public async Task<ActionResult<RoutePlanResponse>> Plan([FromBody] CreateRouteRequest request, CancellationToken cancellationToken)
    {
        try { return Ok(await _routeService.PlanAsync(request, cancellationToken)); }
        catch (KeyNotFoundException exception) { return NotFound(new { message = exception.Message }); }
        catch (InvalidOperationException exception) { return BadRequest(new { message = exception.Message }); }
    }
}
