using StadsLens.Contracts.Routes;

namespace StadsLens.Application.Interfaces.Services;

public interface IRouteService
{
    Task<IEnumerable<RouteResponse>> GetAllAsync(string language = Languages.Default, CancellationToken cancellationToken = default);
    Task<RouteResponse?> GetByIdAsync(int id, string language = Languages.Default, CancellationToken cancellationToken = default);
    Task<RoutePlanResponse> PlanAsync(CreateRouteRequest request, CancellationToken cancellationToken = default);
}
