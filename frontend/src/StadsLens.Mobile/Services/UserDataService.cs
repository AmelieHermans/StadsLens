using System.Net;
using System.Net.Http.Json;
using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.Services;

public class UserDataService : ApiService<User>, IUserDataService
{
    public UserDataService(HttpClient httpClient) : base(httpClient, "api/users")
    {
    }

    public async Task<User?> LoginAsync(string name, string password)
    {
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{_endpoint}/login", new { name, password });
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            return null;

        if (!response.IsSuccessStatusCode)
            throw CreateError(response, "Error_LoginFailed");

        return await response.Content.ReadFromJsonAsync<User>();
    }

    public async Task<User?> RegisterAsync(string name, string password, string language)
    {
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{_endpoint}/register", new { name, password, preferredLanguage = language });
        if (response.StatusCode == HttpStatusCode.Conflict)
            return null;

        if (!response.IsSuccessStatusCode)
            throw CreateError(response, "Error_RegisterFailed");

        return await response.Content.ReadFromJsonAsync<User>()
            ?? throw CreateError(response, "Error_RegisterFailed");
    }
}
