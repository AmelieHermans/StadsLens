namespace StadsLens.Application.Interfaces.AI;

// Zet tekst om naar gesproken audio (Azure AI Speech). De implementatie zit in Infrastructure.
public interface ISpeechService
{
    // Geeft een mp3-bestand terug met de tekst voorgelezen in de gevraagde taal ("nl" of "en").
    Task<byte[]> SynthesizeAsync(string text, string language, CancellationToken cancellationToken = default);
}
