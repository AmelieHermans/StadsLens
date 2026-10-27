using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace StadsLens.Mobile.Services;

public class LocalizationService : ILocalizationService
{
    // Eén gedeelde instantie: de XAML-extensie {loc:Translate} heeft geen toegang tot DI,
    // en in MauiProgram wordt dezelfde instantie als ILocalizationService geregistreerd.
    public static LocalizationService Instance { get; } = new();

    private static readonly ResourceManager Resources =
        new("StadsLens.Mobile.Resources.Strings.AppResources", typeof(LocalizationService).Assembly);

    private CultureInfo culture = new("nl");

    public event PropertyChangedEventHandler? PropertyChanged;

    private LocalizationService()
    {
    }

    public string this[string key] => Resources.GetString(key, culture) ?? key;

    public void SetLanguage(string language)
    {
        culture = new CultureInfo(language);
        CultureInfo.CurrentUICulture = culture;

        // Lege naam = "alles is veranderd": elke binding op de indexer haalt haar tekst opnieuw op.
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
    }
}
