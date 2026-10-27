using StadsLens.Contracts.Landmarks;

namespace StadsLens.Contracts.Routes;

public class RouteResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int LandmarkCount { get; set; }

    // De landmarks in de route, in de juiste volgorde.
    public List<LandmarkResponse> Landmarks { get; set; } = new();
}
