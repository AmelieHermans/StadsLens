namespace StadsLens.Domain.Entities;

public class Visit
{
    public int Id { get; set; }
    public DateTime VisitedAt { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int LandmarkId { get; set; }
    public Landmark Landmark { get; set; } = null!;
}
