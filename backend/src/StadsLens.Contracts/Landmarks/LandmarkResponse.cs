namespace StadsLens.Contracts.Landmarks;

public class LandmarkResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string History { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string CustomVisionTag { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? ImageCredit { get; set; }

    // De taal van Name en History ("nl" of "en"). Valt terug op "nl" als vertalen niet lukt.
    public string Language { get; set; } = "nl";

    // Alle routes waarin deze landmark voorkomt.
    public List<int> RouteIds { get; set; } = [];
}
