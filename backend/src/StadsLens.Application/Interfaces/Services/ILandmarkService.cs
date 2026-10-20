using StadsLens.Contracts.Landmarks;

namespace StadsLens.Application.Interfaces.Services;

public interface ILandmarkService
{
    Task<IEnumerable<LandmarkResponse>> GetByRouteIdAsync(int routeId);
    Task<LandmarkResponse?> GetByIdAsync(int id);
    Task<LandmarkResponse?> GetByCustomVisionTagAsync(string tag);
}
