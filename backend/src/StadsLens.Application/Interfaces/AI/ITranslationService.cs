namespace StadsLens.Application.Interfaces.AI;

// Vertaalt teksten (Azure AI Translator). De implementatie zit in Infrastructure.
public interface ITranslationService
{
    // Geeft de vertalingen in dezelfde volgorde terug als de teksten die je meegeeft.
    Task<IReadOnlyList<string>> TranslateAsync(IReadOnlyList<string> texts, string fromLanguage, string toLanguage, CancellationToken cancellationToken = default);
}
