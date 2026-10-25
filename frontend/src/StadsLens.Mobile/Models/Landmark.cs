namespace StadsLens.Mobile.Models;

public class Landmark
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string History { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string CustomVisionTag { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }

    // Enkel ingevuld wanneer de landmark via een route opgehaald wordt.
    public int? OrderInRoute { get; set; }

    // Alle routes waarin deze landmark voorkomt.
    public List<int> RouteIds { get; set; } = [];

    // Enkel in de app: wordt ingevuld op basis van de bezoeken van de gebruiker.
    public bool IsVisited { get; set; }
}
