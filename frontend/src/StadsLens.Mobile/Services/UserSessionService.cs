using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.Services;

// Houdt de ingelogde gebruiker bij zolang de app draait.
public class UserSessionService : IUserSessionService
{
    private const string LanguageKey = "language";

    public User? CurrentUser { get; private set; }

    public bool IsLoggedIn => CurrentUser is not null;

    public void SetUser(User user) => CurrentUser = user;

    public void Clear() => CurrentUser = null;

    // De taal wordt bewaard op het toestel, zodat ze na het herstarten van de app blijft.
    public string Language { get; private set; } = Preferences.Default.Get(LanguageKey, "nl");

    public void SetLanguage(string language)
    {
        Language = language;
        Preferences.Default.Set(LanguageKey, language);
    }
}
