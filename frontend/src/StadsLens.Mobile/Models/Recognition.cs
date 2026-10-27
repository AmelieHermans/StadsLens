namespace StadsLens.Mobile.Models;

// Het resultaat van een scan (Azure AI Custom Vision in de backend).
public class Recognition
{
    public bool IsRecognized { get; set; }
    public string? Tag { get; set; }

    // Tussen 0 en 1.
    public double Probability { get; set; }

    // Enkel ingevuld als het model zeker genoeg is.
    public Landmark? Landmark { get; set; }
}
