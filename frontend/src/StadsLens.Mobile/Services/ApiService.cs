using System.Net.Http.Json;

namespace StadsLens.Mobile.Services;

// Generieke implementatie van IApiService; elke DataService erft hiervan en geeft zijn eigen endpoint mee.
public abstract class ApiService<T> : IApiService<T> where T : class
{
    protected readonly HttpClient _httpClient;
    protected readonly string _endpoint;

    protected ApiService(HttpClient httpClient, string endpoint)
    {
        _httpClient = httpClient;
        _endpoint = endpoint;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        List<T>? items = await _httpClient.GetFromJsonAsync<List<T>>(_endpoint);
        return items ?? [];
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"{_endpoint}/{id}");
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<T>()
            : null;
    }

    public async Task<T?> CreateAsync(T item)
    {
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_endpoint, item);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<T>()
            : null;
    }

    public async Task<bool> UpdateAsync(int id, T item)
    {
        using HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{_endpoint}/{id}", item);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using HttpResponseMessage response = await _httpClient.DeleteAsync($"{_endpoint}/{id}");
        return response.IsSuccessStatusCode;
    }
}
