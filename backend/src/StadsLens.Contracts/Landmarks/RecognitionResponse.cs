namespace StadsLens.Contracts.Landmarks;

// Het resultaat van een scan. Landmark is enkel ingevuld als het model zeker genoeg is.
public class RecognitionResponse
{
    public bool IsRecognized { get; set; }

    // De beste gok van Custom Vision, ook als die niet zeker genoeg is.
    public string? Tag { get; set; }
    public double Probability { get; set; }

    public LandmarkResponse? Landmark { get; set; }
}
