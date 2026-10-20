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
    public int OrderInRoute { get; set; }
    public int RouteId { get; set; }
}
