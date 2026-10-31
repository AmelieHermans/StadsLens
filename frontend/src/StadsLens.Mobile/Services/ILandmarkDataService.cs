using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.Services;

public interface ILandmarkDataService : IApiService<Landmark>
{
    Task<IEnumerable<Landmark>> GetLandmarksAsync();
    Task<IEnumerable<Landmark>> GetByRouteAsync(int routeId);

    // Naam en geschiedenis in de gevraagde taal ("nl" of "en").
    Task<Landmark?> GetByIdAsync(int id, string language);
    Task<Landmark?> GetByTagAsync(string tag, string language);

    // Stuurt een foto naar de backend, die de bezienswaardigheid herkent.
    Task<Recognition> RecognizeAsync(Stream photo, string fileName, string language);

    // De geschiedenis voorgelezen als mp3.
    Task<byte[]> GetAudioAsync(int id, string language);

    // Een AI-impressie van het gebouw in dit tijdperk ("past" of "future"); geeft het pad naar de afbeelding terug.
    // De foto is optioneel: zonder foto maakt de AI het beeld enkel op basis van de beschrijving.
    Task<string> GetTimeTravelImageAsync(int id, string era, string? photoPath);
}
