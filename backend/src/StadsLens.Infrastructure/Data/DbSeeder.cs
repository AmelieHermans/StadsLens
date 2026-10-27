
using Microsoft.EntityFrameworkCore;
using StadsLens.Domain.Entities;

namespace StadsLens.Infrastructure.Data;

// Vult een lege database met voorbeeldroutes en voegt landmarks toe
// voor Geel, Herentals en Olen zonder hiervoor routes aan te maken.
public static class DbSeeder
{
    public static async Task SeedAsync(StadsLensDbContext context)
    {
        // ============================================================
        // BESTAANDE LANDMARKS VAN MECHELEN
        // ============================================================

        var sintRombouts = new Landmark
        {
            Name = "Sint-Romboutskathedraal",
            History = "De toren van de Sint-Romboutskathedraal werd gebouwd tussen 1452 en 1520 en is 97 meter hoog.",
            Latitude = 51.0289,
            Longitude = 4.4797,
            CustomVisionTag = "sint-rombouts"
        };

        var stadhuis = new Landmark
        {
            Name = "Stadhuis van Mechelen",
            History = "Het stadhuis op de Grote Markt combineert de gotische Lakenhalle met het Paleis van de Grote Raad.",
            Latitude = 51.0280,
            Longitude = 4.4813,
            CustomVisionTag = "stadhuis"
        };

        var hofVanBusleyden = new Landmark
        {
            Name = "Hof van Busleyden",
            History = "Dit renaissancepaleis werd rond 1503 gebouwd voor Hiëronymus van Busleyden, een vriend van Erasmus.",
            Latitude = 51.0302,
            Longitude = 4.4826,
            CustomVisionTag = "hof-van-busleyden"
        };

        var kazerneDossin = new Landmark
        {
            Name = "Kazerne Dossin",
            History = "Vanuit de Dossinkazerne werden tussen 1942 en 1944 meer dan 25.000 Joden en Roma gedeporteerd. Vandaag is het een memoriaal en museum.",
            Latitude = 51.0326,
            Longitude = 4.4749,
            CustomVisionTag = "kazerne-dossin"
        };

        // ============================================================
        // BESTAANDE ROUTES VAN MECHELEN
        // ============================================================

        if (!await context.Routes.AnyAsync(
                r => r.Name == "Historisch Mechelen"))
        {
            var historischMechelen = new Route
            {
                Name = "Historisch Mechelen",
                Description = "Een korte wandeling langs de bekendste gebouwen van het centrum.",
                City = "Mechelen",
                RouteLandmarks =
                {
                    new RouteLandmark
                    {
                        Landmark = sintRombouts,
                        OrderInRoute = 1
                    },
                    new RouteLandmark
                    {
                        Landmark = stadhuis,
                        OrderInRoute = 2
                    },
                    new RouteLandmark
                    {
                        Landmark = hofVanBusleyden,
                        OrderInRoute = 3
                    }
                }
            };

            context.Routes.Add(historischMechelen);
        }

        if (!await context.Routes.AnyAsync(
                r => r.Name == "Van Grote Markt tot Dossin"))
        {
            var groteMarktTotDossin = new Route
            {
                Name = "Van Grote Markt tot Dossin",
                Description = "Start op de Grote Markt en wandel langs de kathedraal naar Kazerne Dossin.",
                City = "Mechelen",
                RouteLandmarks =
                {
                    new RouteLandmark
                    {
                        Landmark = stadhuis,
                        OrderInRoute = 1
                    },
                    new RouteLandmark
                    {
                        Landmark = sintRombouts,
                        OrderInRoute = 2
                    },
                    new RouteLandmark
                    {
                        Landmark = kazerneDossin,
                        OrderInRoute = 3
                    }
                }
            };

            context.Routes.Add(groteMarktTotDossin);
        }

        // Sla de bestaande routes eerst op.
        await context.SaveChangesAsync();

        // ============================================================
        // GEEL - 10 LANDMARKS, GEEN NIEUWE ROUTE
        // ============================================================

        var geelLandmarks = new List<Landmark>
        {
            new()
            {
                Name = "Sint-Amandskerk",
                History = "De Sint-Amandskerk ligt aan de Markt en vormt een belangrijk herkenningspunt in het centrum van Geel.",
                Latitude = 51.1610,
                Longitude = 4.9890,
                CustomVisionTag = "sint-amandskerk"
            },
            new()
            {
                Name = "Sint-Dimpnakerk",
                History = "Deze kerk is verbonden met de verering van Sint-Dimpna en de eeuwenoude traditie van gezinsverpleging in Geel.",
                Latitude = 51.1575,
                Longitude = 4.9970,
                CustomVisionTag = "sint-dimpnakerk"
            },
            new()
            {
                Name = "Markt van Geel",
                History = "De Markt vormt het historische centrum van Geel en is een ontmoetingsplaats voor inwoners en bezoekers.",
                Latitude = 51.1608,
                Longitude = 4.9892,
                CustomVisionTag = "markt-geel"
            },
            new()
            {
                Name = "Gasthuismuseum",
                History = "Het Gasthuismuseum vertelt het verhaal van de zorgtraditie en de geschiedenis van de Geelse gezinsverpleging.",
                Latitude = 51.1584,
                Longitude = 4.9940,
                CustomVisionTag = "gasthuismuseum"
            },
            new()
            {
                Name = "Cultuurcentrum de Werft",
                History = "De Werft is een cultureel centrum waar voorstellingen, concerten en andere culturele activiteiten plaatsvinden.",
                Latitude = 51.1615,
                Longitude = 4.9880,
                CustomVisionTag = "de-werft"
            },
            new()
            {
                Name = "Stadhuis van Geel",
                History = "Het stadhuis is een herkenningspunt van het lokale bestuur in het centrum van Geel.",
                Latitude = 51.1605,
                Longitude = 4.9890,
                CustomVisionTag = "stadhuis-geel"
            },
            new()
            {
                Name = "Station Geel",
                History = "Het station verbindt Geel per spoor met andere steden en vormt een toegangspoort tot de stad.",
                Latitude = 51.1640,
                Longitude = 4.9870,
                CustomVisionTag = "station-geel"
            },
            new()
            {
                Name = "Sint-Franciscuskerk",
                History = "Deze kerk is een religieus herkenningspunt in Geel en maakt deel uit van het plaatselijke kerkelijke erfgoed.",
                Latitude = 51.1620,
                Longitude = 4.9910,
                CustomVisionTag = "sint-franciscuskerk"
            },
            new()
            {
                Name = "Zammelkerk",
                History = "De kerk van Zammel vormt een herkenningspunt in deze deelgemeente van Geel.",
                Latitude = 51.1220,
                Longitude = 4.9340,
                CustomVisionTag = "zammelkerk"
            },
            new()
            {
                Name = "Kerk van Ten Aard",
                History = "De dorpskerk van Ten Aard is een herkenningspunt van deze Geelse deelgemeenschap.",
                Latitude = 51.1810,
                Longitude = 4.9800,
                CustomVisionTag = "kerk-ten-aard"
            }
        };

        // ============================================================
        // HERENTALS - 10 LANDMARKS, GEEN NIEUWE ROUTE
        // ============================================================

        var herentalsLandmarks = new List<Landmark>
        {
            new()
            {
                Name = "Lakenhal",
                History = "De historische Lakenhal herinnert aan het handelsverleden van Herentals en ligt aan de Grote Markt.",
                Latitude = 51.1760,
                Longitude = 4.8350,
                CustomVisionTag = "lakenhal"
            },
            new()
            {
                Name = "Sint-Waldetrudiskerk",
                History = "Deze gotische kerk is een van de belangrijkste historische monumenten van Herentals.",
                Latitude = 51.1765,
                Longitude = 4.8350,
                CustomVisionTag = "sint-waldetrudiskerk"
            },
            new()
            {
                Name = "Begijnhof Herentals",
                History = "Het historische begijnhof getuigt van het religieuze en sociale leven van vroegere generaties.",
                Latitude = 51.1790,
                Longitude = 4.8300,
                CustomVisionTag = "begijnhof-herentals"
            },
            new()
            {
                Name = "Bovenpoort",
                History = "De Bovenpoort is een overblijfsel van de historische stadsverdediging van Herentals.",
                Latitude = 51.1730,
                Longitude = 4.8390,
                CustomVisionTag = "bovenpoort"
            },
            new()
            {
                Name = "Kasteel Le Paige",
                History = "Kasteel Le Paige is een historisch kasteel in een groene omgeving en een bekende bezienswaardigheid van Herentals.",
                Latitude = 51.1780,
                Longitude = 4.8400,
                CustomVisionTag = "kasteel-le-paige"
            },
            new()
            {
                Name = "Hidrodoe",
                History = "Hidrodoe is een interactief waterdoecentrum waar bezoekers de eigenschappen en het belang van water ontdekken.",
                Latitude = 51.1700,
                Longitude = 4.8470,
                CustomVisionTag = "hidrodoe"
            },
            new()
            {
                Name = "Netepark",
                History = "Het Netepark is een recreatiegebied met sport- en ontspanningsmogelijkheden.",
                Latitude = 51.1710,
                Longitude = 4.8470,
                CustomVisionTag = "netepark"
            },
            new()
            {
                Name = "Toeristentoren",
                History = "De Toeristentoren is een uitkijkpunt in de bosrijke omgeving van Herentals.",
                Latitude = 51.1660,
                Longitude = 4.8540,
                CustomVisionTag = "toeristentoren"
            },
            new()
            {
                Name = "Grote Markt Herentals",
                History = "De Grote Markt is het centrale plein van Herentals en wordt omringd door historische gebouwen en horecazaken.",
                Latitude = 51.1760,
                Longitude = 4.8350,
                CustomVisionTag = "grote-markt-herentals"
            },
            new()
            {
                Name = "Begijnhofkerk",
                History = "De begijnhofkerk maakt deel uit van het historische begijnhof en het religieuze erfgoed van Herentals.",
                Latitude = 51.1790,
                Longitude = 4.8310,
                CustomVisionTag = "begijnhofkerk"
            }
        };

        // ============================================================
        // OLEN - 10 LANDMARKS, GEEN NIEUWE ROUTE
        // ============================================================

        var olenLandmarks = new List<Landmark>
        {
            new()
            {
                Name = "Sint-Martinuskerk Olen-Centrum",
                History = "De Sint-Martinuskerk is een herkenningspunt van de historische dorpskern van Olen-Centrum.",
                Latitude = 51.1450,
                Longitude = 4.8590,
                CustomVisionTag = "sint-martinuskerk-olen"
            },
            new()
            {
                Name = "Gemeentehuis Olen",
                History = "Het gemeentehuis is het administratieve centrum van de gemeente Olen.",
                Latitude = 51.1450,
                Longitude = 4.8590,
                CustomVisionTag = "gemeentehuis-olen"
            },
            new()
            {
                Name = "Olen-Centrum",
                History = "De dorpskern van Olen-Centrum vormt een vertrekpunt om het lokale erfgoed en het dorpsleven te ontdekken.",
                Latitude = 51.1450,
                Longitude = 4.8590,
                CustomVisionTag = "olen-centrum"
            },
            new()
            {
                Name = "Olen-Statie",
                History = "Olen-Statie ontwikkelde zich rond de spoorverbinding en vormt een van de herkenbare woonkernen van Olen.",
                Latitude = 51.1610,
                Longitude = 4.9070,
                CustomVisionTag = "olen-statie"
            },
            new()
            {
                Name = "Station Olen",
                History = "Het station biedt een spoorverbinding voor Olen en de omliggende gemeenten.",
                Latitude = 51.1610,
                Longitude = 4.9070,
                CustomVisionTag = "station-olen"
            },
            new()
            {
                Name = "Onze-Lieve-Vrouwkerk van Olen",
                History = "Deze kerk is een religieus herkenningspunt in een van de dorpskernen van Olen.",
                Latitude = 51.1530,
                Longitude = 4.8780,
                CustomVisionTag = "olv-kerk-olen"
            },
            new()
            {
                Name = "Winkelomheide",
                History = "Winkelomheide is een landelijke omgeving in de regio met een eigen dorpskarakter.",
                Latitude = 51.1690,
                Longitude = 4.8810,
                CustomVisionTag = "winkelomheide"
            },
            new()
            {
                Name = "Kerk van Onze-Lieve-Vrouw-Olen",
                History = "De dorpskerk is een herkenningspunt in de kern Onze-Lieve-Vrouw-Olen.",
                Latitude = 51.1530,
                Longitude = 4.8780,
                CustomVisionTag = "kerk-olv-olen"
            },
            new()
            {
                Name = "Kanaal Bocholt-Herentals",
                History = "Het kanaal Bocholt-Herentals is een belangrijke waterweg in de regio en biedt mogelijkheden voor wandelingen langs het water.",
                Latitude = 51.1580,
                Longitude = 4.8860,
                CustomVisionTag = "kanaal-bocholt-herentals"
            },
            new()
            {
                Name = "Groene omgeving van Olen",
                History = "De landelijke omgeving van Olen biedt wandelmogelijkheden langs groene zones en rustige wegen.",
                Latitude = 51.1490,
                Longitude = 4.8750,
                CustomVisionTag = "groene-omgeving-olen"
            }
        };

        // ============================================================
        // SLA NIEUWE LANDMARKS OP ZONDER ROUTES AAN TE MAKEN
        // ============================================================

        await AddLandmarksIfMissingAsync(context, geelLandmarks);
        await AddLandmarksIfMissingAsync(context, herentalsLandmarks);
        await AddLandmarksIfMissingAsync(context, olenLandmarks);

        await context.SaveChangesAsync();
    }

    private static async Task AddLandmarksIfMissingAsync(
        StadsLensDbContext context,
        List<Landmark> landmarks)
    {
        foreach (var landmark in landmarks)
        {
            var exists = await context.Landmarks.AnyAsync(
                l => l.Name == landmark.Name);

            if (!exists)
            {
                context.Landmarks.Add(landmark);
            }
        }
    }
}
