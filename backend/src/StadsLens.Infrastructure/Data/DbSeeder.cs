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
                History = "De Sint-Romboutskathedraal is gewijd aan de Ierse missionaris Sint-Rombout. De bouw van de kerk begon in de 13de eeuw en in 1312 werd ze ingewijd. De eerste steen van de toren werd gelegd in 1452. De bouwmeestersfamilie Keldermans plande een toren van 600 Mechelse voet, ongeveer 167 meter, maar door geldgebrek bleef hij steken op 97 meter. Wie de 538 treden beklimt, kijkt uit over heel Mechelen. In de toren hangen twee beiaarden en hij staat samen met andere belforten op de UNESCO-werelderfgoedlijst. Sinds 1559 is de kerk een kathedraal; vandaag is ze de hoofdkerk van het aartsbisdom Mechelen-Brussel.",
                Latitude = 51.0289,
                Longitude = 4.4790,
                CustomVisionTag = "sint-rombouts"
            },
            new()
            {
                Name = "Stadhuis van Mechelen",
                History = "Het stadhuis aan de oostkant van de Grote Markt bestaat uit drie delen: de lakenhal, het belfort en het Paleis van de Grote Raad. De lakenhal werd in de 14de eeuw gebouwd voor de handel in laken. Het gotische belfort werd nooit afgewerkt zoals gepland en staat op de UNESCO-werelderfgoedlijst. In 1526 begon Rombout II Keldermans aan het paleis voor de Grote Raad, maar door geldnood bleef het bijna 400 jaar onafgewerkt. Pas tussen 1900 en 1911 werd het voltooid volgens de oorspronkelijke plannen. Sinds 1914 doen de gebouwen samen dienst als stadhuis.",
                Latitude = 51.0280,
                Longitude = 4.4811,
                CustomVisionTag = "stadhuis-mechelen"
            },
            new()
            {
                Name = "Hof van Busleyden",
                History = "Dit renaissancepaleis werd in het begin van de 16de eeuw gebouwd voor Hiëronymus van Busleyden, raadsheer van de Grote Raad. Busleyden was bevriend met Erasmus en Thomas More; More logeerde in de zomer van 1515 bij hem in Mechelen. Samen met Erasmus richtte hij het Collegium Trilingue in Leuven op, waar Latijn, Grieks en Hebreeuws werden onderwezen. Vanaf 1620 huisde in het gebouw de Berg van Barmhartigheid: daar kon je tegen een lage rente geld lenen. Sinds 1938 is er een museum gevestigd; vandaag vertelt Museum Hof van Busleyden over Mechelen in de Bourgondische tijd.",
                Latitude = 51.0308,
                Longitude = 4.4821,
                CustomVisionTag = "hof-van-busleyden"
            },
            new()
            {
                Name = "Brusselpoort",
                History = "De Brusselpoort werd in de 13de eeuw gebouwd, kort nadat Mechelen tussen 1264 en 1268 een stadsomwalling kreeg. Ze was de hoogste van de Mechelse stadspoorten en heette daarom ook de Overste Poort. De poort is opgetrokken in Doornikse steen en werd rond 1400 verhoogd met lichtere Balegemse steen. In de 16de eeuw werden de torens verlaagd en kreeg ze haar huidige daken. Toen de stadsmuren werden afgebroken, bleef enkel deze poort staan; op de vrijgekomen ruimte kwamen de boulevards van de kleine ring. Sinds 2010 huist hier Het Firmament, het huis voor figurentheater in Vlaanderen.",
                Latitude = 51.0220,
                Longitude = 4.4738,
                CustomVisionTag = "brusselpoort"
            },
            new()
            {
                Name = "Schepenhuis",
                History = "In 1288 besliste Mechelen om een stenen gebouw op te richten voor de schepenbank, die tot dan in open lucht vergaderde. Het Schepenhuis wedijvert met dat van Aalst om de titel van oudste stadhuis van Vlaanderen. Onder Karel de Stoute werd hier in 1473 het Parlement van Mechelen geïnstalleerd, het hoogste gerechtshof van de Nederlanden. Later zetelde er de Grote Raad, tot die in 1616 verhuisde. Het gebouw was daarna onder meer academie, stedelijk museum en stadsarchief. Sinds 2018 is het het toeristisch infokantoor van Mechelen.",
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
                History = "De Zimmertoren was vroeger de Corneliustoren, een verdedigingstoren van de eerste stadsmuur van Lier die al vóór 1425 bestond. In 1928 schonk uurwerkmaker Louis Zimmer de stad zijn Jubelklok. De vervallen toren werd tussen 1928 en 1930 verbouwd om de klok te huisvesten en op 29 juni 1930 ingehuldigd, honderd jaar na de Belgische onafhankelijkheid. De klok heeft dertien wijzerplaten die onder meer de maanstanden en de getijden tonen. In het museum naast de toren staat ook Zimmers Wonderklok, waarvoor hij felicitaties kreeg van onder anderen Albert Einstein.",
                Latitude = 51.1291,
                Longitude = 4.5702,
                CustomVisionTag = "zimmertoren"
            },
            new()
            {
                Name = "Sint-Gummaruskerk",
                History = "De Sint-Gummaruskerk is gewijd aan de heilige Gummarus van Lier en werd tussen 1378 en 1540 gebouwd in Brabantse gotiek. Aan de bouw werkten onder meer Hendrik Meys, leden van de familie Keldermans en de broers De Waghemakere. Op 20 oktober 1496 werd hier het huwelijk ingezegend van Filips de Schone en Johanna van Castilië, de ouders van keizer Karel V. Binnen vind je een 16de-eeuws doksaal, een barokke preekstoel en een schat aan gotische glasramen. In 1754 kreeg de toren zijn opvallende bekroning.",
                Latitude = 51.1300,
                Longitude = 4.5746,
                CustomVisionTag = "sint-gummaruskerk"
            },
            new()
            {
                Name = "Stadhuis van Lier",
                History = "In 1367 bouwde Lier hier een lakenhal, waarin ook het stadsbestuur zetelde. Vanaf 1418 deed het gebouw alleen nog dienst als stadhuis. In 1740 ontwierp Jan Pieter van Baurscheit de Jonge een bijna volledig nieuw gebouw in Brabantse rococostijl, een van de zeldzame openbare gebouwen in die stijl. Het gotische belfort uit 1369 is 42,5 meter hoog, hoort niet tot het eigenlijke stadhuis en staat op de UNESCO-werelderfgoedlijst.",
                Latitude = 51.1310,
                Longitude = 4.5700,
                CustomVisionTag = "stadhuis-lier"
            },
            new()
            {
                Name = "Gevangenenpoort",
                History = "De Gevangenenpoort, ook binnenste Eikelpoort genoemd, werd opgericht in 1375. Ze is de laatste van de vijf poorten van de eerste stadsomwalling van Lier. Van de 16de eeuw tot 1930 deed ze dienst als stadsgevangenis; de tralies herinneren nog aan die tijd. Oorspronkelijk was de poort gotisch, maar rond 1728 kreeg ze classicistische kenmerken. Boven de doorgang staan beelden van de heilige Rochus en de heilige Margaretha.",
                Latitude = 51.1297,
                Longitude = 4.5689,
                CustomVisionTag = "gevangenenpoort-lier"
            },
            new()
            {
                Name = "Begijnhof Lier",
                History = "Rond 1258 gingen ongehuwde vrouwen samenwonen buiten de eerste stadsmuur van Lier; daaruit groeide een van de drie oudste begijnhoven van Brabant. Begijnen leefden in gemeenschap zonder kloostergeloften en wijdden zich aan gebed en werk. Het Lierse begijnhof is een stratenbegijnhof met smalle straatjes en kleine huisjes. Sinds 1998 staat het samen met twaalf andere Vlaamse begijnhoven op de UNESCO-werelderfgoedlijst. Schrijver Felix Timmermans haalde er veel inspiratie.",
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
                History = "De Lakenhal werd gebouwd toen de lakennijverheid Herentals in de 14de en 15de eeuw welvarend maakte; Herentalse handelaars verkochten hun laken in een groot deel van Europa. Al in de 15de eeuw kreeg het gebouw de functie van stadhuis. In 1512 brandde het bijna volledig af; het werd heropgebouwd en kreeg een toren van ongeveer 35 meter als belfort. Dat belfort staat samen met andere belforten in België en Frankrijk op de UNESCO-werelderfgoedlijst.",
                Latitude = 51.1768,
                Longitude = 4.8362,
                CustomVisionTag = "lakenhal"
            },
            new()
            {
                Name = "Sint-Waldetrudiskerk",
                History = "De Sint-Waldetrudiskerk is een kruiskerk in Brabantse gotiek, vooral gebouwd in de 14de en 15de eeuw. Ongewoon is dat eerst de toren boven de kruising werd gebouwd, en pas daarna het koor. Aan het koor werd vanaf 1417 gewerkt onder leiding van Gillis van den Bossche, die ook bouwmeester was van de kathedraal van Brussel. Het stadsbestuur bracht het retabel van de heiligen Crispinus en Crispinianus tijdig in veiligheid voor de Beeldenstorm van 1566. Later vulden schilders van de Herentalse familie Francken de altaren met hun werk.",
                Latitude = 51.1746,
                Longitude = 4.8357,
                CustomVisionTag = "sint-waldetrudiskerk"
            },
            new()
            {
                Name = "Bovenpoort",
                History = "De Bovenpoort dateert van 1361 en maakte samen met de Zandpoort deel uit van de omwalling van Herentals; het zijn de twee stadspoorten die vandaag nog bestaan. De gevel is bekleed met Gobertangesteen; binnenin werd veel ijzersteen uit de streek gebruikt. In 1754 werd een groot deel van de poort afgebroken omdat ze geen militair nut meer had. Bij de restauratie van 1772 kreeg ze haar huidige uitzicht, met de twee koperen vazen op het dak.",
                Latitude = 51.1736,
                Longitude = 4.8391,
                CustomVisionTag = "bovenpoort"
            },
            new()
            {
                Name = "Begijnhofkerk",
                History = "De Sint-Catharinakerk staat midden in het begijnhof van Herentals. Dat begijnhof werd in 1266 opgericht en was een van de oudste van het hertogdom Brabant. Tijdens de Tachtigjarige Oorlog werd het gesloopt; in 1590 werd het opnieuw opgebouwd aan de Burchtstraat. Het begijnhof bestaat uit één straat met een park, waarin de kerk staat. Begijnen waren vrouwen die in gemeenschap leefden zonder kloostergeloften af te leggen.",
                Latitude = 51.1812,
                Longitude = 4.8374,
                CustomVisionTag = "begijnhofkerk"
            },
            new()
            {
                Name = "Kasteel Le Paige",
                History = "De grond van dit domein hoorde vroeger bij de stadsversterkingen en kwam in 1775 in handen van de familie Le Paige, die er een lusthof van maakte. Een telg van de familie, Henri Le Paige, werd later burgemeester van Herentals. Het kasteel zelf werd in 1892 gebouwd voor Remi Le Paige. Na de dood van de laatste bewoners kwam het in 1976 aan de kerkfabriek, op voorwaarde dat het een kerk zou worden. Omdat dat niet haalbaar was, werd in 2005 in het park een openluchtkerk ingericht: de groene kathedraal. Rond het kasteel ligt een arboretum met tientallen boomsoorten.",
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
                History = "De Sint-Martinuskerk werd in drie fasen gebouwd, en dat zie je nog aan de stenen. De zandstenen westertoren is vroeggotisch en dateert van rond 1400. Het koor in Kempense baksteengotiek komt uit de 15de of 16de eeuw. In 1776 en 1777 kreeg de kerk classicistische zijbeuken. Binnen staan laatgotische houten beelden, barokke altaren en een rococo-preekstoel. De kerk heeft nog een eigen kerkhof, wat zeldzaam is in Vlaanderen, en is sinds 1986 beschermd als monument.",
                Latitude = 51.1443,
                Longitude = 4.8573,
                CustomVisionTag = "sint-martinuskerk-olen"
            },
            new()
            {
                Name = "Gemeentehuis Olen",
                History = "Het gemeentehuis staat aan het Dorp, het driehoekige dorpsplein met lindebomen. Het is een van de weinige Kempense pleinen die hun oorspronkelijke vorm hebben bewaard. De kern van het gebouw gaat terug tot rond 1760. In 1869 kreeg het naar een ontwerp van J. Van Gastel een extra verdieping en werd het een neoclassicistisch dubbelhuis met een driehoekig fronton. Rond het plein staan ook de pastorie, de dorpspomp en het beeld van de Driekantige Boeren van Olen.",
                Latitude = 51.1437,
                Longitude = 4.8596,
                CustomVisionTag = "gemeentehuis-olen"
            },
            new()
            {
                Name = "Sint-Jozefkerk Olen",
                History = "Sint-Jozef-Olen ontstond rond 1912, toen Union Minière in Olen een fabriek bouwde voor de raffinage van ertsen en daarnaast een arbeiderswijk, de Cité. Datzelfde jaar werd het een eigen parochie. De neoromaanse kerk werd in 1923 ontworpen door de architecten C. Veraart en E. Richir en in 1924-1925 gebouwd. Het is een bakstenen zaalkerk met roosvensters. De glasramen werden in 1965-1966 gemaakt door L. Vingerhoets.",
                Latitude = 51.1848,
                Longitude = 4.9044,
                CustomVisionTag = "sint-jozefkerk-olen"
            },
            new()
            {
                Name = "Onze-Lieve-Vrouwkerk van Olen",
                History = "De Onze-Lieve-Vrouwkerk staat in Achter-Olen, het noordelijke deel van Olen dat in 1874 een eigen parochie werd. De neogotische kerk werd in 1865-1866 gebouwd naar een ontwerp van J. Van Gastel en in 1877 ingewijd. Binnen hangt een houten kruisbeeld uit de 14de of 15de eeuw, en het neogotische meubilair werd in 1871 ontworpen door P.J. Taeymans. Rond de kerk ligt een ommuurd kerkhof.",
                Latitude = 51.1522,
                Longitude = 4.8642,
                CustomVisionTag = "olv-kerk-olen"
            },
            new()
            {
                Name = "Buulmolen",
                History = "De Buulmolen wordt al in 1362 vermeld en is genoemd naar het gehucht Hoogbuul. Het is een standerdmolen die graan maalde. Na een brand in 1744 werd hij herbouwd. Voor de aanleg van het Albertkanaal moest hij in 1937 verdwijnen: hij werd ontmanteld en geschonken aan Antwerpen, waar hij op het Noordkasteel kwam te staan. In 1996 kreeg Olen de molen terug, en in 2002-2003 werd hij in zijn 18de-eeuwse uitzicht heropgebouwd in Olen.",
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