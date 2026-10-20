using StadsLens.Domain.Entities;

namespace StadsLens.Application.Interfaces.Repositories;

public interface IRouteRepository
{
    Task<IEnumerable<Route>> GetAllAsync();
    Task<Route?> GetByIdAsync(int id);
}
