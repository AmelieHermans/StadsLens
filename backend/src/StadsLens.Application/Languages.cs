namespace StadsLens.Application;

// De talen die de app ondersteunt. De gegevens in de database staan in het Nederlands.
public static class Languages
{
    public const string Dutch = "nl";
    public const string English = "en";
    public const string French = "fr";
    public const string German = "de";
    public const string Default = Dutch;

    public static readonly string[] All = [Dutch, English, French, German];

    public static bool IsSupported(string? language) => language is not null && All.Contains(language);
}
