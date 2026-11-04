using CommunityToolkit.Mvvm.ComponentModel;

namespace StadsLens.Mobile.Models;

// Eén stap op de schuifregelaar van de tijdreis: een historisch beeld, vandaag of de toekomst.
public class TimeTravelStop : ObservableObject
{
    private string? image;

    public string? Image
    {
        get => image;
        set => SetProperty(ref image, value);
    }

    // Hoe sterk dit beeld doorschijnt: 1 als de schuifregelaar er precies op staat, 0 vanaf de volgende stap.
    private double opacity;

    public double Opacity
    {
        get => opacity;
        set => SetProperty(ref opacity, value);
    }

    public string DateText { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public string Credit { get; set; } = string.Empty;
}
