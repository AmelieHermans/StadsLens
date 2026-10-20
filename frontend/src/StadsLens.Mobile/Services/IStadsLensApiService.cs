using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.Services;

public interface IStadsLensApiService
{
    Task<IEnumerable<Route>> GetRoutesAsync();
    Task<IEnumerable<Landmark>> GetLandmarksByRouteAsync(int routeId);
    Task<RoutePlan?> PlanRouteAsync(int routeId, double distanceKm, double? startLatitude, double? startLongitude);
    Task<Landmark?> GetLandmarkByTagAsync(string tag);
    Task<User?> CreateUserAsync(string name, string preferredLanguage);
    Task<IEnumerable<Visit>> GetVisitsByUserAsync(int userId);
    Task<Visit?> CreateVisitAsync(int userId, int landmarkId);
}
