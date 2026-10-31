namespace StadsLens.Mobile.Models;

public class Route
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int LandmarkCount { get; set; }
    public List<Landmark> Landmarks { get; set; } = [];

    // App-only progress fields populated from the current user's visit log.
    public int VisitedLandmarkCount { get; set; }
    public double Progress => LandmarkCount == 0 ? 0 : (double)VisitedLandmarkCount / LandmarkCount;
    public bool IsComplete => LandmarkCount > 0 && VisitedLandmarkCount >= LandmarkCount;
    public string ProgressText { get; set; } = string.Empty;
}
