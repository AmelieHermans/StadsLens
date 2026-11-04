using System.Net.Http.Json;
using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.Services;

public class LandmarkDataService : ApiService<Landmark>, ILandmarkDataService
{
    public LandmarkDataService(HttpClient httpClient) : base(httpClient, "api/landmarks")
    {
    }

    public async Task<IEnumerable<Landmark>> GetByRouteAsync(int routeId)
    {
        List<Landmark>? landmarks = await _httpClient.GetFromJsonAsync<List<Landmark>>($"api/routes/{routeId}/landmarks");
        return landmarks ?? [];
    }

    public async Task<IEnumerable<Landmark>> GetLandmarksAsync()
    {
        List<Landmark>? landmarks = await _httpClient.GetFromJsonAsync<List<Landmark>>(_endpoint);
        return landmarks ?? [];
    }

    public async Task<Landmark?> GetByIdAsync(int id, string language)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"{_endpoint}/{id}?language={language}");
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<Landmark>()
            : null;
    }

    public async Task<Landmark?> GetByTagAsync(string tag, string language)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"{_endpoint}/by-tag/{Uri.EscapeDataString(tag)}?language={language}");
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<Landmark>()
            : null;
    }

    public async Task<Recognition> RecognizeAsync(Stream photo, string fileName, string language)
    {
        using var content = new MultipartFormDataContent();
        content.Add(new StreamContent(photo), "image", fileName);

        using HttpResponseMessage response = await _httpClient.PostAsync($"{_endpoint}/recognize?language={language}", content);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorMessageAsync(response, "Herkennen is mislukt"));

        return await response.Content.ReadFromJsonAsync<Recognition>() ?? new Recognition();
    }

    public async Task<byte[]> GetAudioAsync(int id, string language)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"{_endpoint}/{id}/audio?language={language}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }

    public async Task<IEnumerable<HistoricalImage>> GetHistoricalImagesAsync(int id, string language)
    {
        List<HistoricalImage>? images = await _httpClient.GetFromJsonAsync<List<HistoricalImage>>($"{_endpoint}/{id}/historical-images?language={language}");
        return images ?? [];
    }

    public async Task<string> GetImagePathAsync(string imageUrl)
    {
        var imagePath = Path.Combine(FileSystem.CacheDirectory, $"landmark_{Path.GetFileName(imageUrl)}");
        if (File.Exists(imagePath))
            return imagePath;

        await File.WriteAllBytesAsync(imagePath, await _httpClient.GetByteArrayAsync(imageUrl));
        return imagePath;
    }

    public async Task<string> GetFutureImageAsync(int id, string? photoPath)
    {
        // Elke afbeelding kost geld en tijd, dus bewaren we ze op het toestel en vragen we ze maar één keer op.
        // De naam van de bronfoto hoort erbij: een nieuwe scan moet een nieuw beeld geven dat op die foto past.
        var source = photoPath is null ? string.Empty : $"_{Path.GetFileNameWithoutExtension(photoPath)}";
        var imagePath = Path.Combine(FileSystem.CacheDirectory, $"timetravel_{id}_future{source}.png");
        if (File.Exists(imagePath))
            return imagePath;

        using var content = new MultipartFormDataContent();

        await using FileStream? photo = photoPath is null ? null : File.OpenRead(photoPath);
        if (photo is not null)
            content.Add(new StreamContent(photo), "image", Path.GetFileName(photoPath)!);

        using HttpResponseMessage response = await _httpClient.PostAsync($"{_endpoint}/{id}/timetravel/future", content);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorMessageAsync(response, "De afbeelding maken is mislukt"));

        await File.WriteAllBytesAsync(imagePath, await response.Content.ReadAsByteArrayAsync());
        return imagePath;
    }
}
