using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.Services;

public interface IUserSessionService
{
    User? CurrentUser { get; }
    bool IsLoggedIn { get; }
    void SetUser(User user);
    void Clear();

    // De foto van de laatste scan per bezienswaardigheid: het "vandaag"-beeld in de tijdreis.
    void SetScanPhoto(int landmarkId, string photoPath);
    string? GetScanPhoto(int landmarkId);

    // De gekozen taal voor de app en de bezienswaardigheden ("nl", "en", "fr" of "de").
    string Language { get; }
    void SetLanguage(string language);

    // Na een scan de geschiedenis meteen voorlezen.
    bool AutoReadAloud { get; }
    void SetAutoReadAloud(bool autoReadAloud);
}
