using Microsoft.AspNetCore.Mvc;
using StadsLens.Application;
using StadsLens.Application.Interfaces.Services;
using StadsLens.Contracts.Landmarks;

namespace StadsLens.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LandmarksController : ControllerBase
{
    private const int MaxImageSize = 4 * 1024 * 1024;

    private readonly ILandmarkService _landmarkService;

    public LandmarksController(ILandmarkService landmarkService)
    {
        _landmarkService = landmarkService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LandmarkResponse>>> GetAllLandmarks([FromQuery] string? language, CancellationToken cancellationToken)
    {
        language = RequestLanguage.Resolve(Request, language);
        if (!Languages.IsSupported(language))
            return UnsupportedLanguage(language);

        return Ok(await _landmarkService.GetAllLandmarks(language, cancellationToken));
    }

    // Naam en geschiedenis in de taal uit ?language= of de Accept-Language-header (Azure AI Translator).
    [HttpGet("{id:int}")]
    public async Task<ActionResult<LandmarkResponse>> GetById(int id, [FromQuery] string? language = null, CancellationToken cancellationToken = default)
    {
        language = RequestLanguage.Resolve(Request, language);
        if (!Languages.IsSupported(language))
            return UnsupportedLanguage(language);

        var landmark = await _landmarkService.GetByIdAsync(id, language, cancellationToken);
        return landmark is null ? NotFound() : Ok(landmark);
    }

    // Wordt gebruikt na een scan: de app stuurt de tag die Custom Vision teruggeeft.
    [HttpGet("by-tag/{tag}")]
    public async Task<ActionResult<LandmarkResponse>> GetByTag(string tag, [FromQuery] string? language = null, CancellationToken cancellationToken = default)
    {
        language = RequestLanguage.Resolve(Request, language);
        if (!Languages.IsSupported(language))
            return UnsupportedLanguage(language);

        var landmark = await _landmarkService.GetByCustomVisionTagAsync(tag, language, cancellationToken);
        return landmark is null ? NotFound() : Ok(landmark);
    }

    // Scan: de app stuurt een foto (multipart, veld "image") en krijgt de herkende landmark terug (Azure AI Custom Vision).
    [HttpPost("recognize")]
    [RequestSizeLimit(MaxImageSize)]
    public async Task<ActionResult<RecognitionResponse>> Recognize(IFormFile image, [FromQuery] string? language = null, CancellationToken cancellationToken = default)
    {
        language = RequestLanguage.Resolve(Request, language);
        if (!Languages.IsSupported(language))
            return UnsupportedLanguage(language);

        if (image.Length == 0)
            return BadRequest(new { message = "De foto is leeg." });

        // Custom Vision weigert afbeeldingen groter dan 4 MB.
        if (image.Length > MaxImageSize)
            return BadRequest(new { message = "De foto is groter dan 4 MB." });

        try
        {
            await using var stream = image.OpenReadStream();
            return Ok(await _landmarkService.RecognizeAsync(stream, language, cancellationToken));
        }
        catch (Exception exception) when (exception is HttpRequestException or InvalidOperationException)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new { message = $"Herkennen is mislukt: {exception.Message}" });
        }
    }

    // De geschiedenis voorgelezen als mp3 (Azure AI Speech).
    [HttpGet("{id:int}/audio")]
    public async Task<IActionResult> GetAudio(int id, [FromQuery] string? language = null, CancellationToken cancellationToken = default)
    {
        language = RequestLanguage.Resolve(Request, language);
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

    // Tijdreis naar het verleden: echte historische beelden, van oud naar recent.
    [HttpGet("{id:int}/historical-images")]
    public async Task<ActionResult<IEnumerable<HistoricalImageResponse>>> GetHistoricalImages(int id, [FromQuery] string? language = null, CancellationToken cancellationToken = default)
    {
        language = RequestLanguage.Resolve(Request, language);
        if (!Languages.IsSupported(language))
            return UnsupportedLanguage(language);

        return Ok(await _landmarkService.GetHistoricalImagesAsync(id, language, cancellationToken));
    }

    // Tijdreis naar de toekomst: een AI-impressie van het gebouw.
    // De foto is optioneel; zonder foto werkt de AI enkel met de beschrijving.
    [HttpPost("{id:int}/timetravel/future")]
    [RequestSizeLimit(MaxImageSize)]
    public async Task<IActionResult> GetTimeTravelImage(int id, IFormFile? image = null, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var photo = image?.OpenReadStream();
            var result = await _landmarkService.GetTimeTravelImageAsync(id, photo, image?.FileName, cancellationToken);
            return result is null ? NotFound() : File(result, "image/png");
        }
        catch (Exception exception) when (exception is HttpRequestException or InvalidOperationException)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new { message = $"De afbeelding maken is mislukt: {exception.Message}" });
        }
    }

    private BadRequestObjectResult UnsupportedLanguage(string language) =>
        BadRequest(new { message = $"Taal '{language}' wordt niet ondersteund. Gebruik {string.Join(", ", Languages.All.Select(code => $"'{code}'"))}." });
}
