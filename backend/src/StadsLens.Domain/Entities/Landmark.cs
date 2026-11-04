namespace StadsLens.Domain.Entities;

public class Landmark
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Geschiedenis in het Nederlands; vertaling gebeurt via Azure AI Translator.
    public string History { get; set; } = string.Empty;

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    // Tagnaam zoals getraind in het Custom Vision-model.
    public string CustomVisionTag { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }
    public string? ImageCredit { get; set; }

    public ICollection<HistoricalImage> HistoricalImages { get; set; } = new List<HistoricalImage>();

    public ICollection<RouteLandmark> RouteLandmarks { get; set; } = new List<RouteLandmark>();

    public ICollection<Visit> Visits { get; set; } = new List<Visit>();
}
