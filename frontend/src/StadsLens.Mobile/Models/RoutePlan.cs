namespace StadsLens.Mobile.Models;

public class RoutePlan
{
    public int RouteId { get; set; }
    public string RouteName { get; set; } = string.Empty;
    public double RequestedDistanceKm { get; set; }
    public double DistanceMeters { get; set; }
    public double DurationSeconds { get; set; }
    public double StartLatitude { get; set; }
    public double StartLongitude { get; set; }
    public bool IsEstimated { get; set; }
    public List<RouteCoordinate> Geometry { get; set; } = [];
    public List<RouteLandmark> Landmarks { get; set; } = [];
}
public class RouteCoordinate { public double Latitude { get; set; } public double Longitude { get; set; } }
public class RouteLandmark { public int Id { get; set; } public string Name { get; set; } = string.Empty; public double Latitude { get; set; } public double Longitude { get; set; } }
