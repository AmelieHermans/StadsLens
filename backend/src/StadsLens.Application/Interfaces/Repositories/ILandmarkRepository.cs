using StadsLens.Domain.Entities;

namespace StadsLens.Application.Interfaces.Repositories;

public interface ILandmarkRepository
{
    Task<IEnumerable<Landmark>> GetByRouteIdAsync(int routeId);
    Task<Landmark?> GetByIdAsync(int id);
    Task<Landmark?> GetByCustomVisionTagAsync(string tag);
}
