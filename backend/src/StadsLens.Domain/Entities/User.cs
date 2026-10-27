namespace StadsLens.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Nooit het wachtwoord zelf bewaren, enkel een gezouten hash (zie IPasswordHasher).
    public string PasswordHash { get; set; } = string.Empty;

    public string PreferredLanguage { get; set; } = "nl";

    public ICollection<Visit> Visits { get; set; } = new List<Visit>();
}
