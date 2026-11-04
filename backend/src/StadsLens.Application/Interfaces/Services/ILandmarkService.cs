using StadsLens.Contracts.Landmarks;
using StadsLens.Domain.Entities;

namespace StadsLens.Application.Interfaces.Services;

public interface ILandmarkService
{
    Task<IEnumerable<LandmarkResponse>> GetByRouteIdAsync(int routeId, string language = Languages.Default, CancellationToken cancellationToken = default);
    Task<IEnumerable<LandmarkResponse>> GetAllLandmarks(string language = Languages.Default, CancellationToken cancellationToken = default);
    Task<LandmarkResponse?> GetByIdAsync(int id, string language = Languages.Default, CancellationToken cancellationToken = default);
    Task<LandmarkResponse?> GetByCustomVisionTagAsync(string tag, string language = Languages.Default, CancellationToken cancellationToken = default);

    // Herkent de bezienswaardigheid op een foto (Custom Vision) en geeft ze terug in de gevraagde taal.
    Task<RecognitionResponse> RecognizeAsync(Stream image, string language = Languages.Default, CancellationToken cancellationToken = default);

    // De geschiedenis voorgelezen als mp3, of null als de landmark niet bestaat.
    Task<byte[]?> GetHistoryAudioAsync(int id, string language = Languages.Default, CancellationToken cancellationToken = default);

    // De echte historische beelden van de landmark, van oud naar recent, met datering en bijschrift in de gevraagde taal.
    Task<IEnumerable<HistoricalImageResponse>> GetHistoricalImagesAsync(int id, string language = Languages.Default, CancellationToken cancellationToken = default);

    // Een AI-impressie van het gebouw in de toekomst (png), of null als de landmark niet bestaat.
    // Met een foto vertrekt de AI van die foto, anders enkel van de beschrijving.
    Task<byte[]?> GetTimeTravelImageAsync(int id, Stream? photo, string? fileName, CancellationToken cancellationToken = default);
}
