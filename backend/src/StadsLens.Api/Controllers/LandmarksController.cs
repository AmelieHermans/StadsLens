using Microsoft.AspNetCore.Mvc;
using StadsLens.Application.Interfaces.Services;
using StadsLens.Contracts.Landmarks;

namespace StadsLens.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LandmarksController : ControllerBase
{
    private readonly ILandmarkService _landmarkService;

    public LandmarksController(ILandmarkService landmarkService)
    {
        _landmarkService = landmarkService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LandmarkResponse>>> GetAllLandmarks()
    {
        var landmarks = await _landmarkService.GetAllLandmarks();
        return Ok(landmarks);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LandmarkResponse>> GetById(int id)
    {
        var landmark = await _landmarkService.GetByIdAsync(id);
        return landmark is null ? NotFound() : Ok(landmark);
    }

    // Wordt gebruikt na een scan: de app stuurt de tag die Custom Vision teruggeeft.
    [HttpGet("by-tag/{tag}")]
    public async Task<ActionResult<LandmarkResponse>> GetByTag(string tag)
    {
        var landmark = await _landmarkService.GetByCustomVisionTagAsync(tag);
        return landmark is null ? NotFound() : Ok(landmark);
    }
}
