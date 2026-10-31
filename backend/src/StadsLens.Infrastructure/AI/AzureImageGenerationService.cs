using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using StadsLens.Application.Interfaces.AI;

namespace StadsLens.Infrastructure.AI;

// Azure OpenAI (afbeeldingsmodel uit de gpt-image-reeks) via de REST API.
public class AzureImageGenerationService : IImageGenerationService
{
    private const string Size = "1536x1024";
    private const string Quality = "medium";

    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AzureImageGenerationService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<byte[]> GenerateAsync(string prompt, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest("generations", out var deployment);
        request.Content = new StringContent(JsonSerializer.Serialize(new { model = deployment, prompt, n = 1, size = Size, quality = Quality }), Encoding.UTF8, "application/json");

        return await SendAsync(request, cancellationToken);
    }

    public async Task<byte[]> EditAsync(Stream image, string fileName, string prompt, CancellationToken cancellationToken = default)
    {
        var imageContent = new StreamContent(image);
        imageContent.Headers.ContentType = new MediaTypeHeaderValue(
            fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ? "image/png" : "image/jpeg");

        using var request = CreateRequest("edits", out var deployment);
        request.Content = new MultipartFormDataContent
        {
            { new StringContent(deployment), "model" },
            { imageContent, "image", fileName },
            { new StringContent(prompt), "prompt" },
            { new StringContent("1"), "n" },
            { new StringContent(Size), "size" },
            { new StringContent(Quality), "quality" }
        };

        return await SendAsync(request, cancellationToken);
    }

    private HttpRequestMessage CreateRequest(string operation, out string deployment)
    {
        var endpoint = _configuration["ImageGeneration:Endpoint"];
        var key = _configuration["ImageGeneration:Key"];
        var configuredDeployment = _configuration["ImageGeneration:Deployment"];

        if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(configuredDeployment))
            throw new InvalidOperationException("ImageGeneration:Endpoint, Key of Deployment ontbreekt in de configuratie (.env).");

        deployment = configuredDeployment;

        var request = new HttpRequestMessage(HttpMethod.Post, $"{endpoint.TrimEnd('/')}/openai/v1/images/{operation}");
        request.Headers.Add("api-key", key);
        return request;
    }

    private async Task<byte[]> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        // De gpt-image-modellen sturen de afbeelding altijd als base64 terug.
        using var document = JsonDocument.Parse(await response.Content.ReadAsStreamAsync(cancellationToken));
        var image = document.RootElement.GetProperty("data")[0].GetProperty("b64_json").GetString()
            ?? throw new InvalidOperationException("Azure OpenAI gaf geen afbeelding terug.");

        return Convert.FromBase64String(image);
    }
}
