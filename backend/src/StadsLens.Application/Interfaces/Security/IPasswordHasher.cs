namespace StadsLens.Application.Interfaces.Security;

// Maakt en controleert wachtwoord-hashes. De implementatie zit in Infrastructure.
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}
