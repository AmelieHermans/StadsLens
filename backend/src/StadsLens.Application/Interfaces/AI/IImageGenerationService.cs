namespace StadsLens.Application.Interfaces.AI;

// Maakt afbeeldingen met AI (Azure OpenAI). De implementatie zit in Infrastructure.
public interface IImageGenerationService
{
    // Maakt een nieuwe afbeelding (png) op basis van enkel een beschrijving.
    Task<byte[]> GenerateAsync(string prompt, CancellationToken cancellationToken = default);

    // Past een bestaande foto aan volgens de beschrijving en geeft het resultaat terug (png).
    Task<byte[]> EditAsync(Stream image, string fileName, string prompt, CancellationToken cancellationToken = default);
}
