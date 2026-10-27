using AutoMapper;
using StadsLens.Application.Interfaces.AI;
using StadsLens.Application.Interfaces.Repositories;
using StadsLens.Application.Interfaces.Services;
using StadsLens.Contracts.Landmarks;

namespace StadsLens.Application.Services;

public class LandmarkService : ILandmarkService
{
    private readonly ILandmarkRepository _landmarkRepository;
    private readonly ITranslationService _translationService;
    private readonly ISpeechService _speechService;
    private readonly IMapper _mapper;

    public LandmarkService(ILandmarkRepository landmarkRepository, ITranslationService translationService, ISpeechService speechService, IMapper mapper)
    {
        _landmarkRepository = landmarkRepository;
        _translationService = translationService;
        _speechService = speechService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LandmarkResponse>> GetByRouteIdAsync(int routeId)
    {
        var landmarks = await _landmarkRepository.GetByRouteIdAsync(routeId);
        return _mapper.Map<IEnumerable<LandmarkResponse>>(landmarks);
    }

    public async Task<IEnumerable<LandmarkResponse>> GetAllLandmarks()
    {
        var landmarks = await _landmarkRepository.GetAllLandmarks();
        return _mapper.Map<IEnumerable<LandmarkResponse>>(landmarks);
    }

    public async Task<LandmarkResponse?> GetByIdAsync(int id, string language = Languages.Default, CancellationToken cancellationToken = default)
    {
        var landmark = await _landmarkRepository.GetByIdAsync(id);
        return landmark is null ? null : await TranslateAsync(_mapper.Map<LandmarkResponse>(landmark), language, cancellationToken);
    }

    public async Task<LandmarkResponse?> GetByCustomVisionTagAsync(string tag, string language = Languages.Default, CancellationToken cancellationToken = default)
    {
        var landmark = await _landmarkRepository.GetByCustomVisionTagAsync(tag);
        return landmark is null ? null : await TranslateAsync(_mapper.Map<LandmarkResponse>(landmark), language, cancellationToken);
    }

    public async Task<byte[]?> GetHistoryAudioAsync(int id, string language = Languages.Default, CancellationToken cancellationToken = default)
    {
        var landmark = await GetByIdAsync(id, language, cancellationToken);
        if (landmark is null)
            return null;

        // Voorlezen in de taal waarin de tekst effectief staat (bij een mislukte vertaling is dat Nederlands).
        return await _speechService.SynthesizeAsync($"{landmark.Name}. {landmark.History}", landmark.Language, cancellationToken);
    }

    // De database is in het Nederlands; voor een andere taal worden naam en geschiedenis vertaald.
    private async Task<LandmarkResponse> TranslateAsync(LandmarkResponse landmark, string language, CancellationToken cancellationToken)
    {
        landmark.Language = Languages.Default;
        if (language == Languages.Default)
            return landmark;

        try
        {
            var translated = await _translationService.TranslateAsync([landmark.Name, landmark.History], Languages.Default, language, cancellationToken);
            landmark.Name = translated[0];
            landmark.History = translated[1];
            landmark.Language = language;
        }
        catch (Exception exception) when (exception is HttpRequestException or InvalidOperationException)
        {
            // Liever de Nederlandse tekst tonen dan een fout; Language blijft dan "nl".
        }

        return landmark;
    }
}
