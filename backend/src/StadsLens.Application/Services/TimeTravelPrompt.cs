namespace StadsLens.Application.Services;

// Stelt de opdracht aan het afbeeldingsmodel samen voor de tijdreis.
public static class TimeTravelPrompt
{
    // Beeldmodellen hebben niets aan een lange tekst; het begin van de geschiedenis geeft genoeg context.
    private const int MaxHistoryLength = 1500;

    public static string Create(string name, string history, string era, bool hasPhoto)
    {
        var context = history.Length > MaxHistoryLength ? history[..MaxHistoryLength] : history;

        var subject = hasPhoto
            ? "Transform this photo. Keep the same building recognizable, with the same camera position and framing."
            : $"Create a realistic street-level view of {name}, a landmark building in Belgium.";

        var scene = era == TimeTravelEras.Past
            ? "Show the building and its surroundings as they plausibly looked in an early period of its history. " +
              "Choose a period from the history below and use fitting materials, street life, clothing and light. " +
              "Remove everything modern, such as cars, signs and street lamps."
            : "Show the building and its surroundings in a plausible future around the year 2100. " +
              "The historic building itself stays preserved. Around it the city has adapted: more greenery, " +
              "climate-adaptive streets, sustainable new buildings and quiet electric transport. Keep it believable, not science fiction.";

        return $"{subject} {scene} Photorealistic. No text, captions or watermarks in the image. " +
               $"Building: {name}. History (in Dutch): {context}";
    }
}
