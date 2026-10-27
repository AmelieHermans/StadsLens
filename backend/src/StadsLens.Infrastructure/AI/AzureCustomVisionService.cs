using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using StadsLens.Application.Interfaces.AI;

namespace StadsLens.Infrastructure.AI;

// Azure AI Custom Vision via de Prediction REST API (v3.0), classificatie met de gepubliceerde iteratie.
public class AzureCustomVisionService : IImageRecognitionService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AzureCustomVisionService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<ImagePrediction?> RecognizeAsync(Stream image, CancellationToken cancellationToken = default)
    {
        var key = _configuration["CustomVision:PredictionKey"];
        var endpoint = _configuration["CustomVision:PredictionEndpoint"];
        var projectId = _configuration["CustomVision:ProjectId"];
        var publishedName = _configuration["CustomVision:PublishedName"];
        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(projectId) || string.IsNullOrWhiteSpace(publishedName))
            throw new InvalidOperationException("De CustomVision-instellingen ontbreken in de configuratie (.env).");

        var url = $"{endpoint.TrimEnd('/')}/customvision/v3.0/Prediction/{projectId}/classify/iterations/{Uri.EscapeDataString(publishedName)}/image";

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("Prediction-Key", key);
        request.Content = new StreamContent(image);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<PredictionResult>(cancellationToken);
        var best = result?.Predictions.MaxBy(prediction => prediction.Probability);
        return best is null ? null : new ImagePrediction(best.TagName, best.Probability);
    }

    private sealed class PredictionResult
    {
        public List<Prediction> Predictions { get; set; } = [];
    }

    private sealed class Prediction
    {
        public string TagName { get; set; } = string.Empty;
        public double Probability { get; set; }
    }
}
