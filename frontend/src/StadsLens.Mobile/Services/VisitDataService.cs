using System.Net.Http.Json;
using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.Services;

public class VisitDataService : ApiService<Visit>, IVisitDataService
{
    public VisitDataService(HttpClient httpClient) : base(httpClient, "api/visits")
    {
    }

    public async Task<IEnumerable<Visit>> GetByUserAsync(int userId)
    {
        List<Visit>? visits = await _httpClient.GetFromJsonAsync<List<Visit>>($"api/users/{userId}/visits");
        return visits ?? [];
    }
}
