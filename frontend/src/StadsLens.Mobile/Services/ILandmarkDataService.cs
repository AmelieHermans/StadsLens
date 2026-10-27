using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.Services;

public interface ILandmarkDataService : IApiService<Landmark>
{
    Task<IEnumerable<Landmark>> GetLandmarksAsync();
    Task<IEnumerable<Landmark>> GetByRouteAsync(int routeId);

    // Naam en geschiedenis in de gevraagde taal ("nl" of "en").
    Task<Landmark?> GetByIdAsync(int id, string language);
    Task<Landmark?> GetByTagAsync(string tag, string language);

    // De geschiedenis voorgelezen als mp3.
    Task<byte[]> GetAudioAsync(int id, string language);
}
