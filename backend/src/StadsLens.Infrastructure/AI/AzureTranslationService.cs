using System.Collections.Concurrent;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using StadsLens.Application.Interfaces.AI;

namespace StadsLens.Infrastructure.AI;

// Azure AI Translator via de REST API (v3).
public class AzureTranslationService : ITranslationService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AzureTranslationService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    // Vertalingen blijven bewaard zolang de API draait: de teksten in de database veranderen zelden,
    // en zo moet een lijst routes of bezoeken niet bij elk verzoek opnieuw naar Azure.
    private static readonly ConcurrentDictionary<(string From, string To, string Text), string> Cache = new();

    public async Task<IReadOnlyList<string>> TranslateAsync(IReadOnlyList<string> texts, string fromLanguage, string toLanguage, CancellationToken cancellationToken = default)
    {
        var missing = texts.Where(text => !string.IsNullOrWhiteSpace(text) && !Cache.ContainsKey((fromLanguage, toLanguage, text))).Distinct().ToList();
        if (missing.Count > 0)
        {
            var translations = await TranslateWithAzureAsync(missing, fromLanguage, toLanguage, cancellationToken);
            for (var i = 0; i < missing.Count; i++)
                Cache[(fromLanguage, toLanguage, missing[i])] = translations[i];
        }

        return texts.Select(text => string.IsNullOrWhiteSpace(text) ? text : Cache[(fromLanguage, toLanguage, text)]).ToList();
    }

    private async Task<IReadOnlyList<string>> TranslateWithAzureAsync(IReadOnlyList<string> texts, string fromLanguage, string toLanguage, CancellationToken cancellationToken)
    {
        var key = _configuration["Translator:Key"];
        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("Translator:Key ontbreekt in de configuratie (.env).");

        var endpoint = _configuration["Translator:Endpoint"] ?? "https://api.cognitive.microsofttranslator.com/";
        var url = $"{endpoint.TrimEnd('/')}/translate?api-version=3.0&from={fromLanguage}&to={toLanguage}";

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("Ocp-Apim-Subscription-Key", key);
        request.Headers.Add("Ocp-Apim-Subscription-Region", _configuration["Translator:Region"] ?? "germanywestcentral");
        request.Content = JsonContent.Create(texts.Select(text => new { Text = TranslationGlossary.Apply(text, toLanguage) }));

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var results = await response.Content.ReadFromJsonAsync<List<TranslateResult>>(cancellationToken) ?? [];
        if (results.Count != texts.Count)
            throw new InvalidOperationException("Azure AI Translator gaf niet voor elke tekst een vertaling terug.");

        return results.Select(result => result.Translations.FirstOrDefault()?.Text ?? string.Empty).ToList();
    }

    private sealed class TranslateResult
    {
        public List<Translation> Translations { get; set; } = [];
    }

    private sealed class Translation
    {
        public string Text { get; set; } = string.Empty;
    }
}
