using Microsoft.EntityFrameworkCore;
using StadsLens.Application.Interfaces.Repositories;
using StadsLens.Domain.Entities;
using StadsLens.Infrastructure.Data;

namespace StadsLens.Infrastructure.Repositories;

public class LandmarkRepository : ILandmarkRepository
{
    private readonly StadsLensDbContext _context;

    public LandmarkRepository(StadsLensDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RouteLandmark>> GetByRouteIdAsync(int routeId)
    {
        return await _context.RouteLandmarks
            .Where(rl => rl.RouteId == routeId)
            .Include(rl => rl.Landmark)
                .ThenInclude(l => l.RouteLandmarks)
            .OrderBy(rl => rl.OrderInRoute)
            // Include loopt terug naar RouteLandmark (voor RouteIds); gewone AsNoTracking staat die cyclus niet toe.
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();
    }

    public async Task<Landmark?> GetByIdAsync(int id)
    {
        return await _context.Landmarks
            .Include(l => l.RouteLandmarks)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<Landmark?> GetByCustomVisionTagAsync(string tag)
    {
        return await _context.Landmarks
            .Include(l => l.RouteLandmarks)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.CustomVisionTag == tag);
    }
}
