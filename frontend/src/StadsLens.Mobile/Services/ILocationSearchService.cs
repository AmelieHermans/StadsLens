using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.Services;

public interface ILocationSearchService
{
    Task<List<LocationSuggestion>> SearchAsync(
        string query,
        CancellationToken cancellationToken = default);
}