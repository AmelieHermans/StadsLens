using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.Services;

public interface ILandmarkDataService : IApiService<Landmark>
{
    Task<IEnumerable<Landmark>> GetLandmarksAsync();
    Task<IEnumerable<Landmark>> GetByRouteAsync(int routeId);
    Task<Landmark?> GetByTagAsync(string tag);
}
