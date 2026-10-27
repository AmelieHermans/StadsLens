namespace StadsLens.Application.Interfaces.AI;

// Herkent een bezienswaardigheid op een foto (Azure AI Custom Vision). De implementatie zit in Infrastructure.
public interface IImageRecognitionService
{
    // De tag met de hoogste kans, of null als het model niets teruggeeft.
    Task<ImagePrediction?> RecognizeAsync(Stream image, CancellationToken cancellationToken = default);
}

// Probability ligt tussen 0 en 1.
public record ImagePrediction(string Tag, double Probability);
