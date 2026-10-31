using System.Text.RegularExpressions;
using StadsLens.Application;

namespace StadsLens.Infrastructure.AI;

// Vaste vertalingen voor Vlaamse begrippen en plaatsnamen die Azure AI Translator anders onvertaald laat
// (bv. "Grote Markt") of letterlijk vertaalt. Ze worden met de "dynamic dictionary"-opmaak van Translator
// in de tekst gezet: <mstrans:dictionary translation="Market Square">Grote Markt</mstrans:dictionary>.
public static partial class TranslationGlossary
{
    private static readonly Dictionary<string, Dictionary<string, string>> Terms = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Grote Markt"] = new() { [Languages.English] = "Market Square", [Languages.French] = "Grand-Place", [Languages.German] = "Großer Markt" },
        // Geen Duitse vaste vertaling: die zou niet verbuigen ("des Großen Rat" i.p.v. "des Großen Rates").
        ["Grote Raad"] = new() { [Languages.English] = "Great Council", [Languages.French] = "Grand Conseil" },
        ["Lakenhalle"] = new() { [Languages.English] = "Cloth Hall", [Languages.French] = "halle aux draps", [Languages.German] = "Tuchhalle" },
        ["Lakenhal"] = new() { [Languages.English] = "Cloth Hall", [Languages.French] = "halle aux draps", [Languages.German] = "Tuchhalle" },
        ["lakenhuis"] = new() { [Languages.English] = "cloth hall", [Languages.French] = "halle aux draps", [Languages.German] = "Tuchhalle" },
        // Zonder vaste vertaling zette Translator er Engels ("Mount of Mercy") in de Duitse tekst.
        ["Berg van Barmhartigheid"] = new() { [Languages.English] = "Mount of Piety", [Languages.French] = "mont-de-piété", [Languages.German] = "Leihhaus" },
        ["Schepenhuis"] = new() { [Languages.English] = "Aldermen's House", [Languages.French] = "maison échevinale", [Languages.German] = "Schöffenhaus" },
        ["begijnhof"] = new() { [Languages.English] = "beguinage", [Languages.French] = "béguinage", [Languages.German] = "Beginenhof" },
        ["begijnen"] = new() { [Languages.English] = "beguines", [Languages.French] = "béguines", [Languages.German] = "Beginen" },
        ["Brabantse hooggotiek"] = new() { [Languages.English] = "Brabantine High Gothic", [Languages.French] = "gothique brabançon", [Languages.German] = "Brabanter Hochgotik" },
        ["Brabantse gotiek"] = new() { [Languages.English] = "Brabantine Gothic", [Languages.French] = "gothique brabançon", [Languages.German] = "Brabanter Gotik" },
        ["Jubelklok"] = new() { [Languages.English] = "Jubilee Clock", [Languages.French] = "horloge du Jubilé", [Languages.German] = "Jubiläumsuhr" },
        ["Gevangenenpoort"] = new() { [Languages.English] = "Prisoners' Gate", [Languages.French] = "porte des Prisonniers", [Languages.German] = "Gefangenentor" },
        ["Zimmertoren"] = new() { [Languages.English] = "Zimmer Tower", [Languages.French] = "tour Zimmer", [Languages.German] = "Zimmerturm" },
        // Eigennamen die niet vertaald mogen worden.
        ["Bovenpoort"] = new() { [Languages.English] = "Bovenpoort", [Languages.French] = "Bovenpoort", [Languages.German] = "Bovenpoort" },
        ["Brusselpoort"] = new() { [Languages.English] = "Brusselpoort", [Languages.French] = "Brusselpoort", [Languages.German] = "Brusselpoort" },
        ["Buulmolen"] = new() { [Languages.English] = "Buulmolen", [Languages.French] = "Buulmolen", [Languages.German] = "Buulmolen" },
        ["Hof van Busleyden"] = new() { [Languages.English] = "Hof van Busleyden", [Languages.French] = "Hof van Busleyden", [Languages.German] = "Hof van Busleyden" }
    };

    // De langste begrippen eerst, zodat "Lakenhalle" niet als "Lakenhal" + "le" gelezen wordt.
    private static readonly Regex TermPattern = new(
        $@"\b({string.Join("|", Terms.Keys.OrderByDescending(term => term.Length).Select(Regex.Escape))})\b",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static string Apply(string text, string toLanguage) =>
        TermPattern.Replace(text, match =>
            Terms[match.Value].TryGetValue(toLanguage, out var translation)
                ? $"<mstrans:dictionary translation=\"{translation}\">{match.Value}</mstrans:dictionary>"
                : match.Value);
}
