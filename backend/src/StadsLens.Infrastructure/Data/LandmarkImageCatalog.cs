// Gegenereerd uit de nagekeken metadata van Wikimedia Commons; de volledige bronnen staan in
// StadsLens.Api/wwwroot/images/landmarks/BRONNEN.md.
using StadsLens.Domain.Entities;

namespace StadsLens.Infrastructure.Data;

// De beelden per landmark. Staat een landmark niet in Historical, dan is er geen gedateerd beeld gevonden
// waarvan gebouw en jaartal vaststaan: dan tonen we niets in plaats van iets te verzinnen.
public static class LandmarkImageCatalog
{
    private const string Folder = "images/landmarks/";

    public static readonly Dictionary<string, (string ImageUrl, string Credit)> Today = new()
    {
        ["Sint-Romboutskathedraal"] = (Folder + "rombouts-vandaag.jpg", "Foto 2016: Romaine, CC0, Wikimedia Commons"),
        ["Stadhuis van Mechelen"] = (Folder + "stadhuis-mechelen-vandaag.jpg", "Foto 2023: Smiley.toerist, CC BY-SA 4.0, Wikimedia Commons"),
        ["Hof van Busleyden"] = (Folder + "busleyden-vandaag.jpg", "Foto 2018: Hof van Busleyden, CC BY-SA 4.0, Wikimedia Commons"),
        ["Brusselpoort"] = (Folder + "brusselpoort-vandaag.jpg", "Foto 2016: Ste.caneva, CC BY-SA 4.0, Wikimedia Commons"),
        ["Schepenhuis"] = (Folder + "schepenhuis-vandaag.jpg", "Foto 2024: Calips, CC BY-SA 4.0, Wikimedia Commons"),
        ["Zimmertoren"] = (Folder + "zimmertoren-vandaag.jpg", "Foto 2024: Calips, CC BY-SA 4.0, Wikimedia Commons"),
        ["Sint-Gummaruskerk"] = (Folder + "gummarus-vandaag.jpg", "Foto 2014: Ivan Dillen, CC BY-SA 3.0, Wikimedia Commons"),
        ["Stadhuis van Lier"] = (Folder + "stadhuis-lier-vandaag.jpg", "Foto 2018: Sally V, CC BY 4.0, Wikimedia Commons"),
        ["Gevangenenpoort"] = (Folder + "gevangenenpoort-vandaag.jpg", "Foto 2010: Torsade de Pointes, CC0, Wikimedia Commons"),
        ["Begijnhof Lier"] = (Folder + "begijnhof-lier-vandaag.jpg", "Foto 2009: Vandevorst, Kris, CC BY 4.0, Wikimedia Commons"),
        ["Lakenhal"] = (Folder + "lakenhal-vandaag.jpg", "Foto 2010: Herman van Wolputte, CC BY-SA 3.0, Wikimedia Commons"),
        ["Sint-Waldetrudiskerk"] = (Folder + "waldetrudis-vandaag.jpg", "Foto 2018: Sally V, CC BY 4.0, Wikimedia Commons"),
        ["Bovenpoort"] = (Folder + "bovenpoort-vandaag.jpg", "Foto 2021: FrDr, CC BY-SA 4.0, Wikimedia Commons"),
        ["Begijnhofkerk"] = (Folder + "begijnhofkerk-vandaag.jpg", "Foto 2009: Vandevorst, Kris, CC BY 4.0, Wikimedia Commons"),
        ["Kasteel Le Paige"] = (Folder + "le-paige-vandaag.jpg", "Foto 2021: FrDr, CC BY-SA 4.0, Wikimedia Commons"),
        ["Sint-Martinuskerk Olen-Centrum"] = (Folder + "martinus-olen-vandaag.jpg", "Foto 2009: Michielverbeek, CC BY-SA 3.0, Wikimedia Commons"),
        ["Gemeentehuis Olen"] = (Folder + "gemeentehuis-olen-vandaag.jpg", "Foto 1998: Vlaamse Gemeenschap, CC BY 4.0, Wikimedia Commons"),
        ["Sint-Jozefkerk Olen"] = (Folder + "jozef-olen-vandaag.jpg", "Foto 2011: Sonuwe, CC BY-SA 3.0, Wikimedia Commons"),
        ["Onze-Lieve-Vrouwkerk van Olen"] = (Folder + "olv-olen-vandaag.jpg", "Foto 2011: Sonuwe, CC BY-SA 3.0, Wikimedia Commons"),
        ["Buulmolen"] = (Folder + "buulmolen-vandaag.jpg", "Foto 2019: Berwaerts, Koen, CC BY 4.0, Wikimedia Commons"),
    };

    public static readonly Dictionary<string, HistoricalImage[]> Historical = new()
    {
        ["Sint-Romboutskathedraal"] =
        [
            new() { Year = 1833, DateLabel = "1833", Caption = "De kathedraal boven de Grote Markt, getekend door Samuel Prout.", ImageUrl = Folder + "rombouts-1833.jpg", Credit = "Litho van Samuel Prout, publiek domein, Wikimedia Commons" },
            new() { Year = 1867, DateLabel = "Zomer 1867", Caption = "De toren met zijn afgeplatte top, uit een fotoalbum over de reizen van Victor Hugo.", ImageUrl = Folder + "rombouts-1867.jpg", Credit = "Foto, CC0, Wikimedia Commons" },
            new() { Year = 1915, DateLabel = "ca. 1915", Caption = "De kathedraal en de Grote Markt op een prentkaart.", ImageUrl = Folder + "rombouts-1915.jpg", Credit = "Prentkaart, publiek domein, Wikimedia Commons" },
            new() { Year = 1935, DateLabel = "1935-1940", Caption = "Vakantiefoto in kleur op de Grote Markt, aan de voet van de toren.", ImageUrl = Folder + "rombouts-1935.jpg", Credit = "Kleurenfoto van Richard (R.) Boske, CC0, Wikimedia Commons" },
        ],
        ["Stadhuis van Mechelen"] =
        [
            new() { Year = 1866, DateLabel = "1866-1870", Caption = "De lakenhal met het standbeeld van Margaretha van Oostenrijk, vóór de voltooiing van het paleis.", ImageUrl = Folder + "stadhuis-mechelen-1866.jpg", Credit = "Stereofoto, CC0, Wikimedia Commons" },
            new() { Year = 1913, DateLabel = "1913", Caption = "Het stadhuis kort na de voltooiing van het Paleis van de Grote Raad.", ImageUrl = Folder + "stadhuis-mechelen-1913.jpg", Credit = "Stereofoto (één helft) van Josep Salvany i Blanch, publiek domein, Wikimedia Commons" },
        ],
        ["Hof van Busleyden"] =
        [
            new() { Year = 1855, DateLabel = "Vóór 1855", Caption = "Het Hof getekend door Jean-Baptiste De Noter (1787-1855).", ImageUrl = Folder + "busleyden-1855.jpg", Credit = "Tekening van Jean-Baptiste De Noter, publiek domein, Wikimedia Commons" },
            new() { Year = 1901, DateLabel = "ca. 1901", Caption = "Het gebouw als Berg van Barmhartigheid (Mont de Piété).", ImageUrl = Folder + "busleyden-1901.jpg", Credit = "Prentkaart, publiek domein, Wikimedia Commons" },
            new() { Year = 1909, DateLabel = "ca. 1909", Caption = "De binnentuin en de arcaden op een ingekleurde prentkaart.", ImageUrl = Folder + "busleyden-1909.jpg", Credit = "Prentkaart, publiek domein, Wikimedia Commons" },
            new() { Year = 1916, DateLabel = "25 maart 1916", Caption = "Het verwoeste Hof na de beschietingen van 1914.", ImageUrl = Folder + "busleyden-1916.jpg", Credit = "Foto, publiek domein, Wikimedia Commons" },
        ],
        ["Brusselpoort"] =
        [
            new() { Year = 1780, DateLabel = "Toestand in 1780", Caption = "De poort zoals ze er in 1780 uitzag, op een prent die later werd gedrukt.", ImageUrl = Folder + "brusselpoort-1780.jpg", Credit = "Prent, publiek domein, Wikimedia Commons" },
            new() { Year = 1903, DateLabel = "ca. 1903", Caption = "De poort op een prentkaart van Vanderauwera & Cie.", ImageUrl = Folder + "brusselpoort-1903.jpg", Credit = "Prentkaart, publiek domein, Wikimedia Commons" },
            new() { Year = 1919, DateLabel = "4 augustus 1919", Caption = "Kleurenfoto van Georges Chevalier voor de Archives de la Planète.", ImageUrl = Folder + "brusselpoort-1919.jpg", Credit = "Autochroom van Chevalier Georges, CC BY-SA 4.0, Wikimedia Commons" },
        ],
        ["Schepenhuis"] =
        [
            new() { Year = 1900, DateLabel = "ca. 1900", Caption = "Het Schepenhuis, toen het oude paleis met het stedelijk museum.", ImageUrl = Folder + "schepenhuis-1900.jpg", Credit = "Prentkaart, publiek domein, Wikimedia Commons" },
        ],
        ["Zimmertoren"] =
        [
            new() { Year = 1936, DateLabel = "ca. 1936", Caption = "De toren met de Jubelklok, enkele jaren na de inhuldiging van 1930.", ImageUrl = Folder + "zimmertoren-1936.jpg", Credit = "Prentkaart, publiek domein, Wikimedia Commons" },
        ],
        ["Sint-Gummaruskerk"] =
        [
            new() { Year = 1910, DateLabel = "1910", Caption = "De zuidkant van de kerk met de toren.", ImageUrl = Folder + "gummarus-1910.jpg", Credit = "Foto, publiek domein, Wikimedia Commons" },
        ],
        ["Stadhuis van Lier"] =
        [
            new() { Year = 1875, DateLabel = "Tussen 1850 en 1899", Caption = "De rococogevel en het belfort op een foto.", ImageUrl = Folder + "stadhuis-lier-1850.jpg", Credit = "Foto, publiek domein, Wikimedia Commons" },
            new() { Year = 1910, DateLabel = "1910", Caption = "Het stadhuis en het belfort aan de Grote Markt.", ImageUrl = Folder + "stadhuis-lier-1910.jpg", Credit = "Foto, publiek domein, Wikimedia Commons" },
        ],
        ["Gevangenenpoort"] =
        [
            new() { Year = 1901, DateLabel = "ca. 1901", Caption = "De poort toen ze nog dienstdeed als stadsgevangenis.", ImageUrl = Folder + "gevangenenpoort-1901.jpg", Credit = "Prentkaart, publiek domein, Wikimedia Commons" },
            new() { Year = 1905, DateLabel = "ca. 1905", Caption = "Stadsbewoners poseren onder de boog van de poort.", ImageUrl = Folder + "gevangenenpoort-1905.jpg", Credit = "Prentkaart, publiek domein, Wikimedia Commons" },
        ],
        ["Begijnhof Lier"] =
        [
            new() { Year = 1908, DateLabel = "ca. 1908", Caption = "Een hoekje van het begijnhof met de kerktoren.", ImageUrl = Folder + "begijnhof-lier-1908.jpg", Credit = "Prentkaart, publiek domein, Wikimedia Commons" },
            new() { Year = 1926, DateLabel = "ca. 1926", Caption = "Een straatje langs de muren van het begijnhof.", ImageUrl = Folder + "begijnhof-lier-1926.jpg", Credit = "Prentkaart, publiek domein, Wikimedia Commons" },
            new() { Year = 1934, DateLabel = "15 maart 1934", Caption = "De hoofdingang van het begijnhof, uit een krantenknipsel.", ImageUrl = Folder + "begijnhof-lier-1934.jpg", Credit = "Foto, publiek domein, Wikimedia Commons" },
        ],
        ["Lakenhal"] =
        [
            new() { Year = 1911, DateLabel = "ca. 1911", Caption = "De Lakenhal met het belfort op de Grote Markt, gefotografeerd door Henri Bertels.", ImageUrl = Folder + "lakenhal-1911.jpg", Credit = "Prentkaart, publiek domein, Wikimedia Commons" },
        ],
        ["Sint-Waldetrudiskerk"] =
        [
            new() { Year = 1900, DateLabel = "Vóór 1901", Caption = "De zuidkant van de kerk, nog zonder de hoge torenspits.", ImageUrl = Folder + "waldetrudis-1900.jpg", Credit = "Glasplaat, publiek domein, Wikimedia Commons" },
            new() { Year = 1910, DateLabel = "ca. 1910", Caption = "De kerk met de hoge torenspits en het kerkhof.", ImageUrl = Folder + "waldetrudis-1910.jpg", Credit = "Prentkaart, publiek domein, Wikimedia Commons" },
        ],
    };
}
