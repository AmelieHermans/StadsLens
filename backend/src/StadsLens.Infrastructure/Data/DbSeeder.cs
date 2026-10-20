using Microsoft.EntityFrameworkCore;
using StadsLens.Domain.Entities;

namespace StadsLens.Infrastructure.Data;

// Vult een lege database met een voorbeeldroute zodat de app meteen iets toont.
public static class DbSeeder
{
    public static async Task SeedAsync(StadsLensDbContext context)
    {
        if (await context.Routes.AnyAsync())
            return;

        var route = new Route
        {
            Name = "Historisch Mechelen",
            Description = "Een korte wandeling langs de bekendste gebouwen van het centrum.",
            City = "Mechelen",
            Landmarks =
            {
                new Landmark
                {
                    Name = "Sint-Romboutskathedraal",
                    History = "De toren van de Sint-Romboutskathedraal werd gebouwd tussen 1452 en 1520 en is 97 meter hoog.",
                    Latitude = 51.0289,
                    Longitude = 4.4797,
                    CustomVisionTag = "sint-rombouts",
                    OrderInRoute = 1
                },
                new Landmark
                {
                    Name = "Stadhuis van Mechelen",
                    History = "Het stadhuis op de Grote Markt combineert de gotische Lakenhalle met het Paleis van de Grote Raad.",
                    Latitude = 51.0280,
                    Longitude = 4.4813,
                    CustomVisionTag = "stadhuis",
                    OrderInRoute = 2
                },
                new Landmark
                {
                    Name = "Hof van Busleyden",
                    History = "Dit renaissancepaleis werd rond 1503 gebouwd voor Hiëronymus van Busleyden, een vriend van Erasmus.",
                    Latitude = 51.0302,
                    Longitude = 4.4826,
                    CustomVisionTag = "hof-van-busleyden",
                    OrderInRoute = 3
                }
            }
        };

        context.Routes.Add(route);
        await context.SaveChangesAsync();
    }
}
