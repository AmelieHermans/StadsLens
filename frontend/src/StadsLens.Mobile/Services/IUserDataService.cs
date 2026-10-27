using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.Services;

public interface IUserDataService : IApiService<User>
{
    // Geeft null terug als de naam of het wachtwoord niet klopt.
    Task<User?> LoginAsync(string name, string password);

    // Geeft null terug als de naam al in gebruik is.
    Task<User?> RegisterAsync(string name, string password, string language);
}
