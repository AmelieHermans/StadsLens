using StadsLens.Contracts.Routes;

namespace StadsLens.Application.Interfaces.Services;

public interface IRouteService
{
    Task<IEnumerable<RouteResponse>> GetAllAsync();
    Task<RouteResponse?> GetByIdAsync(int id);
    Task<RoutePlanResponse> PlanAsync(CreateRouteRequest request, CancellationToken cancellationToken = default);
}
