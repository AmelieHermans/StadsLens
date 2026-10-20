using System.ComponentModel.DataAnnotations;

namespace StadsLens.Contracts.Routes;

public sealed class CreateRouteRequest
{
    [Range(1, int.MaxValue)] public int RouteId { get; set; }
    [Range(0.2, 50)] public double DistanceKm { get; set; }
    public double? StartLatitude { get; set; }
    public double? StartLongitude { get; set; }
}
