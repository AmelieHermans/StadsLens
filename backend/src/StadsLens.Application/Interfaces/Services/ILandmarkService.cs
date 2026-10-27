using StadsLens.Contracts.Landmarks;
using StadsLens.Domain.Entities;

namespace StadsLens.Application.Interfaces.Services;

public interface ILandmarkService
{
    Task<IEnumerable<LandmarkResponse>> GetByRouteIdAsync(int routeId);
    Task<IEnumerable<LandmarkResponse>> GetAllLandmarks();
    Task<LandmarkResponse?> GetByIdAsync(int id);
    Task<LandmarkResponse?> GetByCustomVisionTagAsync(string tag);
}
