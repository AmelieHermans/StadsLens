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

    public async Task<Landmark?> GetByTagAsync(string tag)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"{_endpoint}/by-tag/{Uri.EscapeDataString(tag)}");
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<Landmark>()
            : null;
    }
}
