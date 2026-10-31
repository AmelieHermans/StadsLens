using System.ComponentModel;

namespace StadsLens.Mobile.Services;

// De teksten van de app (knoppen, titels, meldingen) in de gekozen taal, uit Resources/Strings/AppResources*.resx.
public interface ILocalizationService : INotifyPropertyChanged
{
    // De tekst bij een sleutel, bv. this["Login_Button"].
    string this[string key] { get; }

    // Wisselt de taal ("nl", "en", "fr" of "de"); alle gebonden teksten worden meteen bijgewerkt.
    void SetLanguage(string language);
}
