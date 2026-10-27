using System.Security.Cryptography;
using StadsLens.Application.Interfaces.Security;

namespace StadsLens.Infrastructure.Security;

// PBKDF2 met SHA-256 en een willekeurige salt per wachtwoord (ingebouwd in .NET, geen extra package).
// Opgeslagen als "iteraties.salt.hash" (Base64), zodat het aantal iteraties later verhoogd kan worden.
public class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public bool Verify(string password, string passwordHash)
    {
        var parts = passwordHash.Split('.');
        if (parts.Length != 3 || !int.TryParse(parts[0], out var iterations))
            return false;

        var salt = Convert.FromBase64String(parts[1]);
        var expected = Convert.FromBase64String(parts[2]);
        var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);

        // Vaste duur, zodat de responstijd niets verraadt over hoeveel bytes er klopten.
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}
