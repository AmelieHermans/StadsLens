namespace StadsLens.Application;

// De tijdperken die je in de tijdreis kan bekijken.
public static class TimeTravelEras
{
    public const string Past = "past";
    public const string Future = "future";

    public static bool IsSupported(string era) => era is Past or Future;
}
