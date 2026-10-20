using AutoMapper;
using StadsLens.Application.Interfaces.Repositories;
using StadsLens.Application.Interfaces.Services;
using StadsLens.Contracts.Routes;

namespace StadsLens.Application.Services;

public class RouteService : IRouteService
{
    private readonly IRouteRepository _routeRepository;
    private readonly IMapper _mapper;

    public RouteService(IRouteRepository routeRepository, IMapper mapper)
    {
        _routeRepository = routeRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RouteResponse>> GetAllAsync()
    {
        var routes = await _routeRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<RouteResponse>>(routes);
    }

    public async Task<RouteResponse?> GetByIdAsync(int id)
    {
        var route = await _routeRepository.GetByIdAsync(id);
        return route is null ? null : _mapper.Map<RouteResponse>(route);
    }
}
