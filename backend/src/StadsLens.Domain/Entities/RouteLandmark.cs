namespace StadsLens.Domain.Entities;

// Koppeltabel tussen Route en Landmark: een landmark kan in meerdere routes zitten.
// De volgorde hoort bij de koppeling, want dezelfde landmark kan in elke route op een andere plaats staan.
public class RouteLandmark
{
    public int RouteId { get; set; }
    public Route Route { get; set; } = null!;

    public int LandmarkId { get; set; }
    public Landmark Landmark { get; set; } = null!;

    public int OrderInRoute { get; set; }
}
