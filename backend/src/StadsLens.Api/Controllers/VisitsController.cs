using Microsoft.AspNetCore.Mvc;
using StadsLens.Application.Interfaces.Services;
using StadsLens.Contracts.Visits;

namespace StadsLens.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VisitsController : ControllerBase
{
    private readonly IVisitService _visitService;

    public VisitsController(IVisitService visitService)
    {
        _visitService = visitService;
    }

    [HttpPost]
    public async Task<ActionResult<VisitResponse>> Create(CreateVisitRequest request)
    {
        var visit = await _visitService.CreateAsync(request);
        return Created($"api/visits/{visit.Id}", visit);
    }
}
