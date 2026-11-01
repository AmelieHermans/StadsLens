namespace StadsLens.Domain.Entities;

// Een echt historisch beeld van een landmark (foto, prentkaart of prent) met de datering uit de bron.
public class HistoricalImage
{
    public int Id { get; set; }
    public int LandmarkId { get; set; }
    public Landmark Landmark { get; set; } = null!;

    // Enkel voor de volgorde; de datering die de gebruiker ziet staat in DateLabel ("ca. 1903", "Vóór 1901").
    public int Year { get; set; }
    public string DateLabel { get; set; } = string.Empty;

    public string Caption { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Credit { get; set; } = string.Empty;
}
