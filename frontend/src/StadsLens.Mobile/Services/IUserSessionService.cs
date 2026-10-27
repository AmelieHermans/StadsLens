using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.Services;

public interface IUserSessionService
{
    User? CurrentUser { get; }
    bool IsLoggedIn { get; }
    void SetUser(User user);
    void Clear();

    // De gekozen taal voor bezienswaardigheden ("nl" of "en").
    string Language { get; }
    void SetLanguage(string language);
}
