using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.Services;

// Houdt de ingelogde gebruiker bij zolang de app draait.
public class UserSessionService : IUserSessionService
{
    private const string LanguageKey = "language";
    private const string AutoReadAloudKey = "autoReadAloud";

    private readonly ILocalizationService _localization;

    public UserSessionService(ILocalizationService localization)
    {
        _localization = localization;

        // De bewaarde taal meteen toepassen op de teksten van de app.
        _localization.SetLanguage(Language);
    }

    public User? CurrentUser { get; private set; }

    public bool IsLoggedIn => CurrentUser is not null;

    public void SetUser(User user) => CurrentUser = user;

    public void Clear() => CurrentUser = null;

    private readonly Dictionary<int, string> _scanPhotos = new();

    public void SetScanPhoto(int landmarkId, string photoPath) => _scanPhotos[landmarkId] = photoPath;

    public string? GetScanPhoto(int landmarkId) =>
        _scanPhotos.TryGetValue(landmarkId, out var photoPath) && File.Exists(photoPath) ? photoPath : null;

    // De instellingen worden bewaard op het toestel, zodat ze na het herstarten van de app blijven.
    public string Language { get; private set; } = Preferences.Default.Get(LanguageKey, "nl");

    public void SetLanguage(string language)
    {
        Language = language;
        Preferences.Default.Set(LanguageKey, language);
        _localization.SetLanguage(language);
    }

    public bool AutoReadAloud { get; private set; } = Preferences.Default.Get(AutoReadAloudKey, false);

    public void SetAutoReadAloud(bool autoReadAloud)
    {
        AutoReadAloud = autoReadAloud;
        Preferences.Default.Set(AutoReadAloudKey, autoReadAloud);
    }
}
