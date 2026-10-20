using System.Net.Http.Json;
using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.Services;

public class StadsLensApiService : IStadsLensApiService
{
    private readonly HttpClient _httpClient;

    public StadsLensApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Route>> GetRoutesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Route>>("api/routes") ?? [];
    }

    public async Task<IEnumerable<Landmark>> GetLandmarksByRouteAsync(int routeId)
    {
        return await _httpClient.GetFromJsonAsync<List<Landmark>>($"api/routes/{routeId}/landmarks") ?? [];
    }

    public async Task<RoutePlan?> PlanRouteAsync(int routeId, double distanceKm, double? startLatitude, double? startLongitude)
    {
        var response = await _httpClient.PostAsJsonAsync("api/routes/plan", new { routeId, distanceKm, startLatitude, startLongitude });
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<RoutePlan>() : null;
    }

    public async Task<Landmark?> GetLandmarkByTagAsync(string tag)
    {
        var response = await _httpClient.GetAsync($"api/landmarks/by-tag/{Uri.EscapeDataString(tag)}");
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<Landmark>()
            : null;
    }

    public async Task<User?> CreateUserAsync(string name, string preferredLanguage)
    {
        var response = await _httpClient.PostAsJsonAsync("api/users", new { name, preferredLanguage });
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<User>()
            : null;
    }

    public async Task<IEnumerable<Visit>> GetVisitsByUserAsync(int userId)
    {
        return await _httpClient.GetFromJsonAsync<List<Visit>>($"api/users/{userId}/visits") ?? [];
    }

    public async Task<Visit?> CreateVisitAsync(int userId, int landmarkId)
    {
        var response = await _httpClient.PostAsJsonAsync("api/visits", new { userId, landmarkId });
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<Visit>()
            : null;
    }
}
