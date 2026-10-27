using System.ComponentModel.DataAnnotations;

namespace StadsLens.Contracts.Users;

public class LoginRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
