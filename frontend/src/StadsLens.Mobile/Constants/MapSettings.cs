namespace StadsLens.Mobile.Constants;

public static class MapSettings
{
    // CARTO gebruikt OSM-data voor de basemap. De key is publiek zichtbaar in de app,
    // maar beperk hem in het CARTO-dashboard tot de nodige basemap-diensten.
    public const string CartoBasemapApiKey = "cb1_4pu3_1_08440feff77f1e558049691b";

    public static string CartoTileUrl => string.IsNullOrWhiteSpace(CartoBasemapApiKey)
        ? "https://{s}.basemaps.cartocdn.com/rastertiles/voyager/{z}/{x}/{y}{r}.png"
        : $"https://{{s}}.basemaps.cartocdn.com/rastertiles/voyager/{{z}}/{{x}}/{{y}}{{r}}.png?key={CartoBasemapApiKey}";


}
