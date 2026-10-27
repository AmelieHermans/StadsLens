namespace StadsLens.Contracts.Routes;

public sealed class RoutePlanResponse
{
    public int RouteId { get; set; }
    public string RouteName { get; set; } = string.Empty;
    public double RequestedDistanceKm { get; set; }
    public double DistanceMeters { get; set; }
    public double DurationSeconds { get; set; }
    public double StartLatitude { get; set; }
    public double StartLongitude { get; set; }
    public bool IsEstimated { get; set; }
    public IReadOnlyList<RouteCoordinateResponse> Geometry { get; set; } = [];
    public IReadOnlyList<RouteLandmarkResponse> Landmarks { get; set; } = [];
}

public sealed class RouteCoordinateResponse
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public sealed class RouteLandmarkResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int OrderInRoute { get; set; }
}
