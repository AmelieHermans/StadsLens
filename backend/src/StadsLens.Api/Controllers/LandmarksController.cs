using Microsoft.AspNetCore.Mvc;
using StadsLens.Application;
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

    // ?language=en geeft naam en geschiedenis vertaald terug (Azure AI Translator).
    [HttpGet("{id:int}")]
    public async Task<ActionResult<LandmarkResponse>> GetById(int id, [FromQuery] string language = Languages.Default, CancellationToken cancellationToken = default)
    {
        if (!Languages.IsSupported(language))
            return UnsupportedLanguage(language);

        var landmark = await _landmarkService.GetByIdAsync(id, language, cancellationToken);
        return landmark is null ? NotFound() : Ok(landmark);
    }

    // Wordt gebruikt na een scan: de app stuurt de tag die Custom Vision teruggeeft.
    [HttpGet("by-tag/{tag}")]
    public async Task<ActionResult<LandmarkResponse>> GetByTag(string tag, [FromQuery] string language = Languages.Default, CancellationToken cancellationToken = default)
    {
        if (!Languages.IsSupported(language))
            return UnsupportedLanguage(language);

        var landmark = await _landmarkService.GetByCustomVisionTagAsync(tag, language, cancellationToken);
        return landmark is null ? NotFound() : Ok(landmark);
    }

    // De geschiedenis voorgelezen als mp3 (Azure AI Speech).
    [HttpGet("{id:int}/audio")]
    public async Task<IActionResult> GetAudio(int id, [FromQuery] string language = Languages.Default, CancellationToken cancellationToken = default)
    {
        if (!Languages.IsSupported(language))
            return UnsupportedLanguage(language);

        try
        {
            var audio = await _landmarkService.GetHistoryAudioAsync(id, language, cancellationToken);
            return audio is null ? NotFound() : File(audio, "audio/mpeg");
        }
        catch (Exception exception) when (exception is HttpRequestException or InvalidOperationException)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new { message = $"Voorlezen is mislukt: {exception.Message}" });
        }
    }

    private BadRequestObjectResult UnsupportedLanguage(string language) =>
        BadRequest(new { message = $"Taal '{language}' wordt niet ondersteund. Gebruik 'nl' of 'en'." });
}
