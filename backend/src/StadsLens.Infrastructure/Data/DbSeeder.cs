using Microsoft.EntityFrameworkCore;
using StadsLens.Domain.Entities;

namespace StadsLens.Infrastructure.Data;

// Vult de database met 4 duidelijk herkenbare landmarks per stad
// (Mechelen, Geel, Herentals, Olen) en de twee bestaande Mechelse routes.
//
// Elke landmark is één gebouw met een eigen silhouet dat goed zichtbaar is
// vanaf straatniveau, zodat Custom Vision het betrouwbaar kan herkennen.
// Pleinen, dorpskernen, natuurgebieden en stations zitten er bewust niet meer in.
public static class DbSeeder
{
    // Landmarks uit een vorige versie van de seeder die niet meer gebruikt worden.
    // Ze worden verwijderd zolang ze in geen enkele route zitten.
    private static readonly string[] ObsoleteLandmarkNames =
    {
        "Markt van Geel", "Gasthuismuseum", "Cultuurcentrum de Werft", "Station Geel",
        "Sint-Franciscuskerk", "Zammelkerk", "Kerk van Ten Aard",
        "Begijnhof Herentals", "Kasteel Le Paige", "Hidrodoe", "Netepark",
        "Toeristentoren", "Grote Markt Herentals",
        "Olen-Centrum", "Olen-Statie", "Station Olen", "Winkelomheide",
        "Kerk van Onze-Lieve-Vrouw-Olen", "Kanaal Bocholt-Herentals",
        "Groene omgeving van Olen"
    };

    public static async Task SeedAsync(StadsLensDbContext context)
    {
        // ============================================================
        // MECHELEN - 4 LANDMARKS
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
                Name = "Kazerne Dossin",
                History = "Vanuit de Dossinkazerne werden tussen 1942 en 1944 meer dan 25.000 Joden en Roma gedeporteerd. Vandaag is het een memoriaal en museum.",
                Latitude = 51.0342,
                Longitude = 4.4785,
                CustomVisionTag = "kazerne-dossin"
            }
        };

        // ============================================================
        // GEEL - 4 LANDMARKS
        // ============================================================

        var geel = new List<Landmark>
        {
            new()
            {
                Name = "Sint-Dimpnakerk",
                History = "Deze gotische kerk werd vanaf 1349 gebouwd op de plek waar Sint-Dimpna vereerd wordt. Uit de opvang van pelgrims bij deze kerk groeide de Geelse gezinsverpleging.",
                Latitude = 51.1626,
                Longitude = 5.0021,
                CustomVisionTag = "sint-dimpnakerk"
            },
            new()
            {
                Name = "Sint-Amandskerk",
                History = "De oudste parochiekerk van Geel werd grotendeels gebouwd tussen 1490 en 1531 in laatgotische stijl. De toren is ongeveer 59 meter hoog.",
                Latitude = 51.1620,
                Longitude = 4.9905,
                CustomVisionTag = "sint-amandskerk"
            },
            new()
            {
                Name = "Stadhuis van Geel",
                History = "Het stadhuis begon als lakenhal in de vroege 15de eeuw en werd later ingericht als stadhuis. Het oudste deel van het huidige gebouw dateert uit de 17de eeuw.",
                Latitude = 51.1614,
                Longitude = 4.9870,
                CustomVisionTag = "stadhuis-geel"
            },
            new()
            {
                Name = "Campus Thomas More Geel",
                History = "Geel is de hoofdzetel van hogeschool Thomas More Kempen, die in 1995 ontstond als Katholieke Hogeschool Kempen en sinds 2012 de naam Thomas More draagt.",
                Latitude = 51.1609,
                Longitude = 4.9614,
                CustomVisionTag = "campus-thomas-more-geel"
            }
        };

        // ============================================================
        // HERENTALS - 4 LANDMARKS
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
            }
        };

        // ============================================================
        // OLEN - 4 LANDMARKS
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
            }
        };

        // ============================================================
        // LANDMARKS OPSLAAN OF BIJWERKEN
        // ============================================================

        await RemoveObsoleteLandmarksAsync(context);

        await UpsertLandmarksAsync(context, mechelen);
        await UpsertLandmarksAsync(context, geel);
        await UpsertLandmarksAsync(context, herentals);
        await UpsertLandmarksAsync(context, olen);

        await context.SaveChangesAsync();

        // ============================================================
        // ROUTES VAN MECHELEN
        // ============================================================
        // De landmarks komen uit de database, zodat er geen dubbele
        // landmarks ontstaan wanneer de seeder opnieuw draait.

        var sintRombouts = await GetLandmarkAsync(context, "Sint-Romboutskathedraal");
        var stadhuis = await GetLandmarkAsync(context, "Stadhuis van Mechelen");
        var hofVanBusleyden = await GetLandmarkAsync(context, "Hof van Busleyden");
        var kazerneDossin = await GetLandmarkAsync(context, "Kazerne Dossin");

        if (!await context.Routes.AnyAsync(r => r.Name == "Historisch Mechelen"))
        {
            context.Routes.Add(new Route
            {
                Name = "Historisch Mechelen",
                Description = "Een korte wandeling langs de bekendste gebouwen van het centrum.",
                City = "Mechelen",
                RouteLandmarks =
                {
                    new RouteLandmark { Landmark = sintRombouts, OrderInRoute = 1 },
                    new RouteLandmark { Landmark = stadhuis, OrderInRoute = 2 },
                    new RouteLandmark { Landmark = hofVanBusleyden, OrderInRoute = 3 }
                }
            });
        }

        if (!await context.Routes.AnyAsync(r => r.Name == "Van Grote Markt tot Dossin"))
        {
            context.Routes.Add(new Route
            {
                Name = "Van Grote Markt tot Dossin",
                Description = "Start op de Grote Markt en wandel langs de kathedraal naar Kazerne Dossin.",
                City = "Mechelen",
                RouteLandmarks =
                {
                    new RouteLandmark { Landmark = stadhuis, OrderInRoute = 1 },
                    new RouteLandmark { Landmark = sintRombouts, OrderInRoute = 2 },
                    new RouteLandmark { Landmark = kazerneDossin, OrderInRoute = 3 }
                }
            });
        }

        await context.SaveChangesAsync();
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

    // Verwijdert landmarks uit de vorige versie, behalve als ze in een route gebruikt worden.
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