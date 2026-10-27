using System.Security;
using System.Text;
using Microsoft.Extensions.Configuration;
using StadsLens.Application;
using StadsLens.Application.Interfaces.AI;

namespace StadsLens.Infrastructure.AI;

// Azure AI Speech (text-to-speech) via de REST API.
public class AzureSpeechService : ISpeechService
{
    // Per taal een neurale stem: Vlaams voor Nederlands, Brits voor Engels, Belgisch voor Frans, Duits voor Duits.
    private static readonly Dictionary<string, (string Locale, string Voice)> Voices = new()
    {
        [Languages.Dutch] = ("nl-BE", "nl-BE-DenaNeural"),
        [Languages.English] = ("en-GB", "en-GB-SoniaNeural"),
        [Languages.French] = ("fr-BE", "fr-BE-CharlineNeural"),
        [Languages.German] = ("de-DE", "de-DE-KatjaNeural")
    };

    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AzureSpeechService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<byte[]> SynthesizeAsync(string text, string language, CancellationToken cancellationToken = default)
    {
        var key = _configuration["Speech:Key"];
        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("Speech:Key ontbreekt in de configuratie (.env).");

        var region = _configuration["Speech:Region"] ?? "germanywestcentral";
        var (locale, voice) = Voices.TryGetValue(language, out var found) ? found : Voices[Languages.Default];

        // SSML: de tekst moet ge-escaped worden zodat tekens als & en < geen ongeldige XML geven.
        var ssml = $"<speak version='1.0' xml:lang='{locale}'><voice name='{voice}'>{SecurityElement.Escape(text)}</voice></speak>";

        using var request = new HttpRequestMessage(HttpMethod.Post, $"https://{region}.tts.speech.microsoft.com/cognitiveservices/v1");
        request.Headers.Add("Ocp-Apim-Subscription-Key", key);
        request.Headers.Add("X-Microsoft-OutputFormat", "audio-24khz-48kbitrate-mono-mp3");
        request.Headers.Add("User-Agent", "StadsLens");
        request.Content = new StringContent(ssml, Encoding.UTF8, "application/ssml+xml");

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsByteArrayAsync(cancellationToken);
    }
}
