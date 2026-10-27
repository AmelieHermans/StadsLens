
namespace StadsLens.Mobile.Models;

public class GeneratedRoute
{
    public double DistanceKm { get; set; }

    public double DurationMinutes { get; set; }

    public string MapHtml { get; set; } = string.Empty;

    public List<Landmark> Landmarks { get; set; } = new();
}
