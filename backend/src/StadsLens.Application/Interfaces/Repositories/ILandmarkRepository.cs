using StadsLens.Domain.Entities;

namespace StadsLens.Application.Interfaces.Repositories;

public interface ILandmarkRepository
{
    // Geeft de koppelingen terug (met Landmark ingeladen) zodat de volgorde binnen de route mee komt.
    Task<IEnumerable<RouteLandmark>> GetByRouteIdAsync(int routeId);
    Task<IEnumerable<Landmark>> GetAllLandmarks();
    Task<Landmark?> GetByIdAsync(int id);
    Task<Landmark?> GetByCustomVisionTagAsync(string tag);
}
