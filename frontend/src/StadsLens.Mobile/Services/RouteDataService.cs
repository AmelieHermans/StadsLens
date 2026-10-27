using System.Net.Http.Json;
using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.Services;

public class RouteDataService : ApiService<Route>, IRouteDataService
{
    public RouteDataService(HttpClient httpClient) : base(httpClient, "api/routes")
    {
    }

    public async Task<RoutePlan?> PlanRouteAsync(int routeId, double distanceKm, double? startLatitude, double? startLongitude)
    {
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{_endpoint}/plan", new { routeId, distanceKm, startLatitude, startLongitude });
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<RoutePlan>()
            : null;
    }
}
