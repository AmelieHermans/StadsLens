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

    // De echte historische beelden van het gebouw, van oud naar recent.
    Task<IEnumerable<HistoricalImage>> GetHistoricalImagesAsync(int id, string language);

    // Haalt een beeld van de backend op en bewaart het op het toestel; geeft het pad naar het bestand terug.
    // Via de eigen HttpClient, want een rechtstreekse afbeeldingslink stuurt de Ngrok-header niet mee.
    Task<string> GetImagePathAsync(string imageUrl);

    // Een AI-impressie van het gebouw in de toekomst; geeft het pad naar de afbeelding terug.
    // De foto is optioneel: zonder foto maakt de AI het beeld enkel op basis van de beschrijving.
    Task<string> GetFutureImageAsync(int id, string? photoPath);
}
