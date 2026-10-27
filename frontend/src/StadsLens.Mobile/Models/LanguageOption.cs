namespace StadsLens.Mobile.Models;

// Een taal die de gebruiker kan kiezen in de instellingen.
public class LanguageOption
{
    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;

    public static List<LanguageOption> All { get; } =
    [
        new() { Code = "nl", DisplayName = "Nederlands" },
        new() { Code = "en", DisplayName = "English" }
    ];
}
