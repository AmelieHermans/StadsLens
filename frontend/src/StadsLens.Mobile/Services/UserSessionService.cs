using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.Services;

// Houdt de ingelogde gebruiker bij zolang de app draait.
public class UserSessionService : IUserSessionService
{
    public User? CurrentUser { get; private set; }

    public bool IsLoggedIn => CurrentUser is not null;

    public void SetUser(User user) => CurrentUser = user;

    public void Clear() => CurrentUser = null;
}
