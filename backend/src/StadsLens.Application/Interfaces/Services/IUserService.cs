using StadsLens.Contracts.Users;

namespace StadsLens.Application.Interfaces.Services;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAllAsync();
    Task<UserResponse?> GetByIdAsync(int id);

    // Geeft null terug als de naam al in gebruik is.
    Task<UserResponse?> RegisterAsync(RegisterRequest request);

    // Geeft null terug als de naam niet bestaat of het wachtwoord fout is.
    Task<UserResponse?> LoginAsync(LoginRequest request);
}
