namespace StadsLens.Application;

// De talen die de app ondersteunt. De gegevens in de database staan in het Nederlands.
public static class Languages
{
    public const string Dutch = "nl";
    public const string English = "en";
    public const string Default = Dutch;

    public static bool IsSupported(string? language) => language is Dutch or English;
}
