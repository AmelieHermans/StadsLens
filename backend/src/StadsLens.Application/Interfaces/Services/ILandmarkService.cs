using StadsLens.Contracts.Landmarks;
using StadsLens.Domain.Entities;

namespace StadsLens.Application.Interfaces.Services;

public interface ILandmarkService
{
    Task<IEnumerable<LandmarkResponse>> GetByRouteIdAsync(int routeId);
    Task<IEnumerable<LandmarkResponse>> GetAllLandmarks();
    Task<LandmarkResponse?> GetByIdAsync(int id, string language = Languages.Default, CancellationToken cancellationToken = default);
    Task<LandmarkResponse?> GetByCustomVisionTagAsync(string tag, string language = Languages.Default, CancellationToken cancellationToken = default);

    // De geschiedenis voorgelezen als mp3, of null als de landmark niet bestaat.
    Task<byte[]?> GetHistoryAudioAsync(int id, string language = Languages.Default, CancellationToken cancellationToken = default);
}
