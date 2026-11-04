using AutoMapper;
using StadsLens.Application.Interfaces.AI;
using StadsLens.Application.Interfaces.Repositories;
using StadsLens.Application.Interfaces.Services;
using StadsLens.Contracts.Landmarks;

namespace StadsLens.Application.Services;

public class LandmarkService : ILandmarkService
{
    // Onder deze kans tonen we liever "niet herkend" dan een verkeerde bezienswaardigheid.
    public const double MinimumProbability = 0.6;

    private readonly ILandmarkRepository _landmarkRepository;
    private readonly ContentTranslator _translator;
    private readonly ISpeechService _speechService;
    private readonly IImageRecognitionService _imageRecognitionService;
    private readonly IImageGenerationService _imageGenerationService;
    private readonly IMapper _mapper;

    public LandmarkService(ILandmarkRepository landmarkRepository, ContentTranslator translator, ISpeechService speechService, IImageRecognitionService imageRecognitionService, IImageGenerationService imageGenerationService, IMapper mapper)
    {
        _landmarkRepository = landmarkRepository;
        _translator = translator;
        _speechService = speechService;
        _imageRecognitionService = imageRecognitionService;
        _imageGenerationService = imageGenerationService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LandmarkResponse>> GetByRouteIdAsync(int routeId, string language = Languages.Default, CancellationToken cancellationToken = default)
    {
        var landmarks = await _landmarkRepository.GetByRouteIdAsync(routeId);
        return await TranslateAsync(_mapper.Map<List<LandmarkResponse>>(landmarks), language, cancellationToken);
    }

    public async Task<IEnumerable<LandmarkResponse>> GetAllLandmarks(string language = Languages.Default, CancellationToken cancellationToken = default)
    {
        var landmarks = await _landmarkRepository.GetAllLandmarks();
        return await TranslateAsync(_mapper.Map<List<LandmarkResponse>>(landmarks), language, cancellationToken);
    }

    public async Task<LandmarkResponse?> GetByIdAsync(int id, string language = Languages.Default, CancellationToken cancellationToken = default)
    {
        var landmark = await _landmarkRepository.GetByIdAsync(id);
        return landmark is null ? null : (await TranslateAsync([_mapper.Map<LandmarkResponse>(landmark)], language, cancellationToken))[0];
    }

    public async Task<LandmarkResponse?> GetByCustomVisionTagAsync(string tag, string language = Languages.Default, CancellationToken cancellationToken = default)
    {
        var landmark = await _landmarkRepository.GetByCustomVisionTagAsync(tag);
        return landmark is null ? null : (await TranslateAsync([_mapper.Map<LandmarkResponse>(landmark)], language, cancellationToken))[0];
    }

    public async Task<RecognitionResponse> RecognizeAsync(Stream image, string language = Languages.Default, CancellationToken cancellationToken = default)
    {
        var prediction = await _imageRecognitionService.RecognizeAsync(image, cancellationToken);
        var response = new RecognitionResponse
        {
            Tag = prediction?.Tag,
            Probability = prediction?.Probability ?? 0
        };

        if (prediction is null || prediction.Probability < MinimumProbability)
            return response;

        response.Landmark = await GetByCustomVisionTagAsync(prediction.Tag, language, cancellationToken);
        response.IsRecognized = response.Landmark is not null;
        return response;
    }

    public async Task<byte[]?> GetHistoryAudioAsync(int id, string language = Languages.Default, CancellationToken cancellationToken = default)
    {
        var landmark = await GetByIdAsync(id, language, cancellationToken);
        if (landmark is null)
            return null;

        // Voorlezen in de taal waarin de tekst effectief staat (bij een mislukte vertaling is dat Nederlands).
        return await _speechService.SynthesizeAsync($"{landmark.Name}. {landmark.History}", landmark.Language, cancellationToken);
    }

    public async Task<IEnumerable<HistoricalImageResponse>> GetHistoricalImagesAsync(int id, string language = Languages.Default, CancellationToken cancellationToken = default)
    {
        var images = _mapper.Map<List<HistoricalImageResponse>>(await _landmarkRepository.GetHistoricalImagesAsync(id));

        var texts = images.SelectMany(image => new[] { image.DateLabel, image.Caption }).ToList();
        var translated = await _translator.TranslateAsync(texts, language, cancellationToken);
        if (translated is null)
            return images;

        for (var i = 0; i < images.Count; i++)
        {
            images[i].DateLabel = translated[2 * i];
            images[i].Caption = translated[2 * i + 1];
        }

        return images;
    }

    public async Task<byte[]?> GetTimeTravelImageAsync(int id, Stream? photo, string? fileName, CancellationToken cancellationToken = default)
    {
        // Rechtstreeks uit de database: de prompt hoeft niet vertaald te worden.
        var landmark = await _landmarkRepository.GetByIdAsync(id);
        if (landmark is null)
            return null;

        var prompt = TimeTravelPrompt.Create(landmark.Name, landmark.History, hasPhoto: photo is not null);
        return photo is null
            ? await _imageGenerationService.GenerateAsync(prompt, cancellationToken)
            : await _imageGenerationService.EditAsync(photo, fileName ?? "photo.jpg", prompt, cancellationToken);
    }

    // De database is in het Nederlands; voor een andere taal worden naam en geschiedenis vertaald,
    // voor alle landmarks samen in één verzoek. Mislukt het, dan blijft alles Nederlands en Language "nl".
    private async Task<List<LandmarkResponse>> TranslateAsync(List<LandmarkResponse> landmarks, string language, CancellationToken cancellationToken)
    {
        var texts = landmarks.SelectMany(landmark => new[] { landmark.Name, landmark.History }).ToList();
        var translated = await _translator.TranslateAsync(texts, language, cancellationToken);

        for (var i = 0; i < landmarks.Count; i++)
        {
            landmarks[i].Language = translated is null ? Languages.Default : language;
            if (translated is null)
                continue;

            landmarks[i].Name = translated[2 * i];
            landmarks[i].History = translated[2 * i + 1];
        }

        return landmarks;
    }
}
