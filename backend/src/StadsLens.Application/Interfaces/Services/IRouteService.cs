using StadsLens.Contracts.Routes;

namespace StadsLens.Application.Interfaces.Services;

public interface IRouteService
{
    Task<IEnumerable<RouteResponse>> GetAllAsync();
    Task<RouteResponse?> GetByIdAsync(int id);
}
