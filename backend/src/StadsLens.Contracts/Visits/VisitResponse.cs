namespace StadsLens.Contracts.Visits;

public class VisitResponse
{
    public int Id { get; set; }
    public DateTime VisitedAt { get; set; }
    public int UserId { get; set; }
    public int LandmarkId { get; set; }
    public string LandmarkName { get; set; } = string.Empty;
}
