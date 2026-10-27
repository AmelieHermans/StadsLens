using System.Net.Http.Headers;

namespace StadsLens.Mobile.Services;

// Zet bij elk verzoek naar de API de gekozen taal in de Accept-Language-header.
// Zo krijgt de app routes, landmarks en bezoeken altijd vertaald terug, zonder ?language= bij elke aanroep.
public class LanguageHandler : DelegatingHandler
{
    private readonly IUserSessionService _userSession;

    public LanguageHandler(IUserSessionService userSession)
    {
        _userSession = userSession;
        InnerHandler = new HttpClientHandler();
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.AcceptLanguage.Clear();
        request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(_userSession.Language));
        return base.SendAsync(request, cancellationToken);
    }
}
