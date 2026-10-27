using Microsoft.EntityFrameworkCore;
using StadsLens.Domain.Entities;

namespace StadsLens.Infrastructure.Data;

// Vult een lege database met voorbeeldroutes zodat de app meteen iets toont.
public static class DbSeeder
{
    public static async Task SeedAsync(StadsLensDbContext context)
    {
        if (await context.Routes.AnyAsync())
            return;

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

        var historischMechelen = new Route
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
        };

        // Deelt de kathedraal en het stadhuis met de route hierboven, maar in een andere volgorde.
        var groteMarktTotDossin = new Route
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
        };

        context.Routes.AddRange(historischMechelen, groteMarktTotDossin);
        await context.SaveChangesAsync();
    }
}
