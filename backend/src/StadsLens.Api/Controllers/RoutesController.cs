using Microsoft.AspNetCore.Mvc;
using StadsLens.Application;
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

    // De taal komt uit ?language= of uit de Accept-Language-header van de app (zie RequestLanguage).
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RouteResponse>>> GetAll([FromQuery] string? language, CancellationToken cancellationToken)
    {
        language = RequestLanguage.Resolve(Request, language);
        if (!Languages.IsSupported(language))
            return UnsupportedLanguage(language);

        return Ok(await _routeService.GetAllAsync(language, cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RouteResponse>> GetById(int id, [FromQuery] string? language, CancellationToken cancellationToken)
    {
        language = RequestLanguage.Resolve(Request, language);
        if (!Languages.IsSupported(language))
            return UnsupportedLanguage(language);

        var route = await _routeService.GetByIdAsync(id, language, cancellationToken);
        return route is null ? NotFound() : Ok(route);
    }

    [HttpGet("{id:int}/landmarks")]
    public async Task<ActionResult<IEnumerable<LandmarkResponse>>> GetLandmarks(int id, [FromQuery] string? language, CancellationToken cancellationToken)
    {
        language = RequestLanguage.Resolve(Request, language);
        if (!Languages.IsSupported(language))
            return UnsupportedLanguage(language);

        return Ok(await _landmarkService.GetByRouteIdAsync(id, language, cancellationToken));
    }

    [HttpPost("plan")]
    public async Task<ActionResult<RoutePlanResponse>> Plan([FromBody] CreateRouteRequest request, CancellationToken cancellationToken)
    {
        try { return Ok(await _routeService.PlanAsync(request, cancellationToken)); }
        catch (KeyNotFoundException exception) { return NotFound(new { message = exception.Message }); }
        catch (InvalidOperationException exception) { return BadRequest(new { message = exception.Message }); }
    }

    private BadRequestObjectResult UnsupportedLanguage(string language) =>
        BadRequest(new { message = $"Taal '{language}' wordt niet ondersteund. Gebruik {string.Join(", ", Languages.All.Select(code => $"'{code}'"))}." });
}
