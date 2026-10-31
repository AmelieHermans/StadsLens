using StadsLens.Application.Interfaces.AI;

namespace StadsLens.Application.Services;

// Vertaalt Nederlandse teksten uit de database naar de gevraagde taal, voor routes, landmarks en bezoeken.
// Lukt het vertalen niet, dan krijg je null terug en toont de app liever de Nederlandse tekst dan een fout.
public class ContentTranslator
{
    private readonly ITranslationService _translationService;

    public ContentTranslator(ITranslationService translationService)
    {
        _translationService = translationService;
    }

    public async Task<IReadOnlyList<string>?> TranslateAsync(IReadOnlyList<string> texts, string language, CancellationToken cancellationToken = default)
    {
        if (language == Languages.Default || texts.Count == 0)
            return texts;

        try
        {
            return await _translationService.TranslateAsync(texts, Languages.Default, language, cancellationToken);
        }
        catch (Exception exception) when (exception is HttpRequestException or InvalidOperationException)
        {
            return null;
        }
    }
}
