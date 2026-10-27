namespace StadsLens.Mobile.Models;

public class Visit
{
    public int Id { get; set; }
    public DateTime VisitedAt { get; set; }
    public int UserId { get; set; }
    public int LandmarkId { get; set; }
    public string LandmarkName { get; set; } = string.Empty;

    // De API stuurt UTC; tonen in de tijdzone van het toestel.
    public DateTime VisitedAtLocal => VisitedAt.ToLocalTime();
}
