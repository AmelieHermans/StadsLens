namespace StadsLens.Mobile.Models;

public class HistoricalImage
{
    public int Year { get; set; }
    public string DateLabel { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Credit { get; set; } = string.Empty;
}
