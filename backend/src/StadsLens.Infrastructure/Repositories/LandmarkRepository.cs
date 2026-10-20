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

    public async Task<IEnumerable<Landmark>> GetByRouteIdAsync(int routeId)
    {
        return await _context.Landmarks
            .Where(l => l.RouteId == routeId)
            .OrderBy(l => l.OrderInRoute)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Landmark?> GetByIdAsync(int id)
    {
        return await _context.Landmarks
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<Landmark?> GetByCustomVisionTagAsync(string tag)
    {
        return await _context.Landmarks
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.CustomVisionTag == tag);
    }
}
