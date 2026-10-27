using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.Services;

public class LocationSearchService : ILocationSearchService
{
    private readonly HttpClient _httpClient;

    public LocationSearchService()
    {
        _httpClient = new HttpClient();

        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "StadsLens-Mobile/1.0 (adreszoeken)");
    }

    public async Task<List<LocationSuggestion>> SearchAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Trim().Length < 3)
            return new();

        var url =
            "https://photon.komoot.io/api/?q=" +
            Uri.EscapeDataString(query.Trim()) +
            "&limit=5&lang=nl";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Accept.ParseAdd("application/json");

        using var response = await _httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        using var json = await response.Content.ReadFromJsonAsync<JsonDocument>(
            cancellationToken: cancellationToken);

        var suggestions = new List<LocationSuggestion>();

        if (json is null ||
            !json.RootElement.TryGetProperty("features", out var features))
            return suggestions;

        foreach (var feature in features.EnumerateArray())
        {
            if (!feature.TryGetProperty("geometry", out var geometry) ||
                !geometry.TryGetProperty("coordinates", out var coordinates) ||
                coordinates.GetArrayLength() < 2)
                continue;

            var longitude = coordinates[0].GetDouble();
            var latitude = coordinates[1].GetDouble();

            if (!feature.TryGetProperty("properties", out var properties))
                continue;

            var parts = new List<string>();

            Add("name");
            Add("housenumber");
            Add("street");
            Add("postcode");
            Add("city");
            Add("country");

            void Add(string key)
            {
                if (properties.TryGetProperty(key, out var value) &&
                    value.ValueKind == JsonValueKind.String)
                {
                    var text = value.GetString();

                    if (!string.IsNullOrWhiteSpace(text) &&
                        !parts.Contains(text))
                    {
                        parts.Add(text);
                    }
                }
            }

            if (parts.Count == 0)
                continue;

            suggestions.Add(new LocationSuggestion
            {
                Label = string.Join(", ", parts),
                Latitude = latitude,
                Longitude = longitude
            });
        }

        return suggestions;
    }
}
