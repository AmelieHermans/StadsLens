namespace StadsLens.Contracts.Users;

public class UserResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PreferredLanguage { get; set; } = "nl";
}
