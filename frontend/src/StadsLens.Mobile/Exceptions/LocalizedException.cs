using StadsLens.Mobile.Services;

namespace StadsLens.Mobile.Exceptions;

// Een fout met een tekst uit Resources/Strings, zodat de gebruiker ze in de taal van de app te zien krijgt.
public class LocalizedException : Exception
{
    public LocalizedException(string key, params object[] values)
        : base(Format(key, values))
    {
    }

    private static string Format(string key, object[] values)
    {
        var text = LocalizationService.Instance[key];
        return values.Length == 0 ? text : string.Format(text, values);
    }
}
