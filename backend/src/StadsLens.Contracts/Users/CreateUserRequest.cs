namespace StadsLens.Contracts.Users;

public class CreateUserRequest
{
    public string Name { get; set; } = string.Empty;
    public string PreferredLanguage { get; set; } = "nl";
}
