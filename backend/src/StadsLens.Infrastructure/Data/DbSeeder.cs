using Microsoft.EntityFrameworkCore;
using StadsLens.Domain.Entities;

namespace StadsLens.Infrastructure.Data;

// Vult de database met 5 landmarks per stad (Mechelen, Lier, Herentals en Olen)
// en wandelroutes voor Mechelen en Lier.
//
// Elke landmark is één gebouw met een eigen silhouet dat goed zichtbaar is vanaf de straat,
// zodat Custom Vision het betrouwbaar kan leren herkennen. Mechelen en Lier hebben genoeg
// foto's op Wikimedia Commons; voor Herentals en Olen worden ze aangevuld met eigen foto's.
public static class DbSeeder
{
    // Landmarks uit vorige versies van de seeder. Ze worden verwijderd
    // zolang ze in geen enkele route meer zitten.
    private static readonly string[] ObsoleteLandmarkNames =
    {
        "Kazerne Dossin",
        // Geel
        "Sint-Amandskerk", "Sint-Dimpnakerk", "Markt van Geel", "Gasthuismuseum",
        "Cultuurcentrum de Werft", "Stadhuis van Geel", "Station Geel",
        "Sint-Franciscuskerk", "Zammelkerk", "Kerk van Ten Aard", "Campus Thomas More Geel",
        // Herentals
        "Begijnhof Herentals", "Hidrodoe", "Netepark", "Toeristentoren", "Grote Markt Herentals",
        // Olen
        "Olen-Centrum", "Olen-Statie", "Station Olen", "Winkelomheide",
        "Kerk van Onze-Lieve-Vrouw-Olen", "Kanaal Bocholt-Herentals", "Groene omgeving van Olen"
    };

    // Routes uit vorige versies die vervangen zijn.
    private static readonly string[] ObsoleteRouteNames =
    {
        "Van Grote Markt tot Dossin"
    };

    public static async Task SeedAsync(StadsLensDbContext context)
    {
        // ============================================================
        // MECHELEN - 5 LANDMARKS
        // ============================================================

        var mechelen = new List<Landmark>
        {
            new()
            {
                Name = "Sint-Romboutskathedraal",
                History = "De toren van de Sint-Romboutskathedraal werd gebouwd tussen 1452 en 1520 en is 97 meter hoog.",
                Latitude = 51.0289,
                Longitude = 4.4790,
                CustomVisionTag = "sint-rombouts"
            },
            new()
            {
                Name = "Stadhuis van Mechelen",
                History = "Het stadhuis op de Grote Markt combineert de gotische Lakenhalle met het Paleis van de Grote Raad.",
                Latitude = 51.0280,
                Longitude = 4.4811,
                CustomVisionTag = "stadhuis-mechelen"
            },
            new()
            {
                Name = "Hof van Busleyden",
                History = "Dit renaissancepaleis werd rond 1503 gebouwd voor Hiëronymus van Busleyden, een vriend van Erasmus.",
                Latitude = 51.0308,
                Longitude = 4.4821,
                CustomVisionTag = "hof-van-busleyden"
            },
            new()
            {
                Name = "Brusselpoort",
                History = "De Brusselpoort dateert uit de 13de eeuw en is de enige overgebleven van de twaalf stadspoorten van Mechelen. In de 16de eeuw werden de torens verlaagd en kreeg ze haar huidige daken.",
                Latitude = 51.0220,
                Longitude = 4.4738,
                CustomVisionTag = "brusselpoort"
            },
            new()
            {
                Name = "Schepenhuis",
                History = "Het Schepenhuis geldt als het eerste stenen stadhuis van Vlaanderen. In de 15de eeuw was het de zetel van de Grote Raad; vandaag huist er de toeristische dienst.",
                Latitude = 51.0276,
                Longitude = 4.4794,
                CustomVisionTag = "schepenhuis-mechelen"
            }
        };

        // ============================================================
        // LIER - 5 LANDMARKS
        // ============================================================

        var lier = new List<Landmark>
        {
            new()
            {
                Name = "Zimmertoren",
                History = "Deze toren hoorde bij de 14de-eeuwse stadsversterking. In 1930 schonk uurwerkmaker Louis Zimmer de Jubelklok met 13 wijzerplaten, die onder meer de maanstanden en de getijden tonen.",
                Latitude = 51.1291,
                Longitude = 4.5702,
                CustomVisionTag = "zimmertoren"
            },
            new()
            {
                Name = "Sint-Gummaruskerk",
                History = "De bouw van deze kerk begon in 1378 en duurde bijna twee eeuwen. Ze is een van de gaafste voorbeelden van Brabantse hooggotiek; de toren kreeg in 1754 zijn rococokoepel.",
                Latitude = 51.1300,
                Longitude = 4.5746,
                CustomVisionTag = "sint-gummaruskerk"
            },
            new()
            {
                Name = "Stadhuis van Lier",
                History = "Het vroegere lakenhuis werd in 1740 herbouwd in Brabantse rococostijl. Het gotische belfort is in het gebouw opgenomen.",
                Latitude = 51.1310,
                Longitude = 4.5700,
                CustomVisionTag = "stadhuis-lier"
            },
            new()
            {
                Name = "Gevangenenpoort",
                History = "Deze gotische stadspoort uit 1375 is het enige restant van de oudste stadsomwalling van Lier. Van de 16de eeuw tot 1930 deed ze dienst als gevangenis.",
                Latitude = 51.1297,
                Longitude = 4.5689,
                CustomVisionTag = "gevangenenpoort-lier"
            },
            new()
            {
                Name = "Begijnhof Lier",
                History = "Het begijnhof van Lier is een van de grootste van Vlaanderen en maakt deel uit van het UNESCO-werelderfgoed van de Vlaamse begijnhoven. De hoofdingang ligt in de Begijnhofstraat.",
                Latitude = 51.1282,
                Longitude = 4.5681,
                CustomVisionTag = "begijnhof-lier"
            }
        };

        // ============================================================
        // HERENTALS - 5 LANDMARKS
        // ============================================================

        var herentals = new List<Landmark>
        {
            new()
            {
                Name = "Lakenhal",
                History = "De Lakenhal werd in de vroege 15de eeuw gebouwd door wolwevers en lakenmakers. Na een brand in 1512 werd ze heropgebouwd; het belfort staat op de UNESCO-werelderfgoedlijst.",
                Latitude = 51.1768,
                Longitude = 4.8362,
                CustomVisionTag = "lakenhal"
            },
            new()
            {
                Name = "Sint-Waldetrudiskerk",
                History = "Deze gotische kerk werd vanaf 1417 gebouwd en is sinds 1936 een beschermd monument.",
                Latitude = 51.1746,
                Longitude = 4.8357,
                CustomVisionTag = "sint-waldetrudiskerk"
            },
            new()
            {
                Name = "Bovenpoort",
                History = "De Bovenpoort is een van de twee overgebleven stadspoorten van Herentals en herinnert aan de vroegere stadsomwalling.",
                Latitude = 51.1736,
                Longitude = 4.8391,
                CustomVisionTag = "bovenpoort"
            },
            new()
            {
                Name = "Begijnhofkerk",
                History = "De Sint-Catharinakerk staat midden in het begijnhof van Herentals, dat deel uitmaakt van het UNESCO-werelderfgoed van de Vlaamse begijnhoven.",
                Latitude = 51.1812,
                Longitude = 4.8374,
                CustomVisionTag = "begijnhofkerk"
            },
            new()
            {
                Name = "Kasteel Le Paige",
                History = "Het kasteel werd in 1892 gebouwd voor de familie Le Paige, die meerdere burgemeesters van Herentals leverde. Rond het kasteel ligt een arboretum met meer dan zeventig boomsoorten.",
                Latitude = 51.1861,
                Longitude = 4.8380,
                CustomVisionTag = "kasteel-le-paige"
            }
        };

        // ============================================================
        // OLEN - 5 LANDMARKS
        // ============================================================

        var olen = new List<Landmark>
        {
            new()
            {
                Name = "Sint-Martinuskerk Olen-Centrum",
                History = "De Sint-Martinuskerk is de parochiekerk van Olen-Centrum en vormt samen met het gemeentehuis het hart van het dorp.",
                Latitude = 51.1443,
                Longitude = 4.8573,
                CustomVisionTag = "sint-martinuskerk-olen"
            },
            new()
            {
                Name = "Gemeentehuis Olen",
                History = "Het gemeentehuis aan het Dorp is het bestuurlijke centrum van Olen.",
                Latitude = 51.1437,
                Longitude = 4.8596,
                CustomVisionTag = "gemeentehuis-olen"
            },
            new()
            {
                Name = "Sint-Jozefkerk Olen",
                History = "Deze neoromaanse kerk werd in 1923 gebouwd voor de arbeiders van de nabijgelegen metaalfabriek. De bouw liep vertraging op door de Eerste Wereldoorlog.",
                Latitude = 51.1848,
                Longitude = 4.9044,
                CustomVisionTag = "sint-jozefkerk-olen"
            },
            new()
            {
                Name = "Onze-Lieve-Vrouwkerk van Olen",
                History = "Deze kerk is toegewijd aan Onze-Lieve-Vrouw en is een van de parochiekerken van Olen.",
                Latitude = 51.1522,
                Longitude = 4.8642,
                CustomVisionTag = "olv-kerk-olen"
            },
            new()
            {
                Name = "Buulmolen",
                History = "Deze windmolen werd afgebroken bij de aanleg van het Albertkanaal en jarenlang in Antwerpen bewaard. Na zijn restauratie keerde hij in 2003 terug naar zijn oorspronkelijke plek in Olen.",
                Latitude = 51.1570,
                Longitude = 4.8593,
                CustomVisionTag = "buulmolen"
            }
        };

        // ============================================================
        // OPKUISEN EN LANDMARKS OPSLAAN OF BIJWERKEN
        // ============================================================

        await RemoveObsoleteRoutesAsync(context);
        await context.SaveChangesAsync();

        await RemoveObsoleteLandmarksAsync(context);
        await UpsertLandmarksAsync(context, mechelen);
        await UpsertLandmarksAsync(context, lier);
        await UpsertLandmarksAsync(context, herentals);
        await UpsertLandmarksAsync(context, olen);
        await context.SaveChangesAsync();

        // ============================================================
        // ROUTES
        // ============================================================
        // De landmarks komen uit de database, zodat er geen dubbele
        // landmarks ontstaan wanneer de seeder opnieuw draait.

        var sintRombouts = await GetLandmarkAsync(context, "Sint-Romboutskathedraal");
        var stadhuisMechelen = await GetLandmarkAsync(context, "Stadhuis van Mechelen");
        var hofVanBusleyden = await GetLandmarkAsync(context, "Hof van Busleyden");
        var brusselpoort = await GetLandmarkAsync(context, "Brusselpoort");

        var zimmertoren = await GetLandmarkAsync(context, "Zimmertoren");
        var sintGummarus = await GetLandmarkAsync(context, "Sint-Gummaruskerk");
        var stadhuisLier = await GetLandmarkAsync(context, "Stadhuis van Lier");
        var gevangenenpoort = await GetLandmarkAsync(context, "Gevangenenpoort");

        await AddRouteIfMissingAsync(context, "Historisch Mechelen",
            "Een korte wandeling langs de bekendste gebouwen van het centrum.",
            "Mechelen", sintRombouts, stadhuisMechelen, hofVanBusleyden);

        await AddRouteIfMissingAsync(context, "Van de kathedraal tot de Brusselpoort",
            "Start aan de Sint-Romboutskathedraal, steek de Grote Markt over en wandel naar de laatste stadspoort van Mechelen.",
            "Mechelen", sintRombouts, stadhuisMechelen, brusselpoort);

        await AddRouteIfMissingAsync(context, "Historisch Lier",
            "Een wandeling van de Grote Markt langs de oude stadsversterking naar de Sint-Gummaruskerk.",
            "Lier", stadhuisLier, gevangenenpoort, zimmertoren, sintGummarus);

        await context.SaveChangesAsync();
    }

    private static async Task AddRouteIfMissingAsync(
        StadsLensDbContext context,
        string name,
        string description,
        string city,
        params Landmark[] landmarksInOrder)
    {
        if (await context.Routes.AnyAsync(r => r.Name == name))
        {
            return;
        }

        var route = new Route
        {
            Name = name,
            Description = description,
            City = city
        };

        for (var i = 0; i < landmarksInOrder.Length; i++)
        {
            route.RouteLandmarks.Add(new RouteLandmark
            {
                Landmark = landmarksInOrder[i],
                OrderInRoute = i + 1
            });
        }

        context.Routes.Add(route);
    }

    // Voegt nieuwe landmarks toe en werkt bestaande bij (op naam),
    // zodat gecorrigeerde coördinaten en teksten ook in een bestaande database terechtkomen.
    private static async Task UpsertLandmarksAsync(
        StadsLensDbContext context,
        List<Landmark> landmarks)
    {
        foreach (var landmark in landmarks)
        {
            var existing = await context.Landmarks
                .FirstOrDefaultAsync(l => l.Name == landmark.Name);

            if (existing is null)
            {
                context.Landmarks.Add(landmark);
            }
            else
            {
                existing.History = landmark.History;
                existing.Latitude = landmark.Latitude;
                existing.Longitude = landmark.Longitude;
                existing.CustomVisionTag = landmark.CustomVisionTag;
            }
        }
    }

    private static async Task RemoveObsoleteRoutesAsync(StadsLensDbContext context)
    {
        var routes = await context.Routes
            .Include(r => r.RouteLandmarks)
            .Where(r => ObsoleteRouteNames.Contains(r.Name))
            .ToListAsync();

        foreach (var route in routes)
        {
            context.Set<RouteLandmark>().RemoveRange(route.RouteLandmarks);
            context.Routes.Remove(route);
        }
    }

    // Verwijdert landmarks uit vorige versies, behalve als ze nog in een route gebruikt worden.
    private static async Task RemoveObsoleteLandmarksAsync(StadsLensDbContext context)
    {
        var obsolete = await context.Landmarks
            .Where(l => ObsoleteLandmarkNames.Contains(l.Name))
            .ToListAsync();

        foreach (var landmark in obsolete)
        {
            var usedInRoute = await context.Set<RouteLandmark>()
                .AnyAsync(rl => rl.Landmark.Name == landmark.Name);

            if (!usedInRoute)
            {
                context.Landmarks.Remove(landmark);
            }
        }
    }

    private static async Task<Landmark> GetLandmarkAsync(StadsLensDbContext context, string name)
    {
        return await context.Landmarks.FirstAsync(l => l.Name == name);
    }
}