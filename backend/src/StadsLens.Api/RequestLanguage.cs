using StadsLens.Application;

namespace StadsLens.Api;

// Bepaalt in welke taal de API antwoordt: ?language=... wint, anders de Accept-Language-header
// die de app bij elk verzoek meestuurt, anders Nederlands.
public static class RequestLanguage
{
    public static string Resolve(HttpRequest request, string? queryLanguage)
    {
        if (!string.IsNullOrWhiteSpace(queryLanguage))
            return queryLanguage;

        // Bv. "de" of "fr-BE,fr;q=0.9": de eerste ondersteunde taal telt.
        foreach (var header in request.GetTypedHeaders().AcceptLanguage.OrderByDescending(value => value.Quality ?? 1))
        {
            var code = header.Value.Value?.Split('-')[0].ToLowerInvariant();
            if (Languages.IsSupported(code))
                return code!;
        }

        return Languages.Default;
    }
}
