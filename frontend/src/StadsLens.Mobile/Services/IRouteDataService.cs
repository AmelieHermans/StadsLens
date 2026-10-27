using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.Services;

public interface IRouteDataService : IApiService<Route>
{
    Task<RoutePlan?> PlanRouteAsync(int routeId, double distanceKm, double? startLatitude, double? startLongitude);
}
