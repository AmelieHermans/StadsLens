using System.ComponentModel.DataAnnotations;

namespace StadsLens.Contracts.Users;

public class RegisterRequest
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    public string PreferredLanguage { get; set; } = "nl";
}
