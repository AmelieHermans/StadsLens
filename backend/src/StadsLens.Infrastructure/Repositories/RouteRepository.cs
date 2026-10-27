using Microsoft.EntityFrameworkCore;
using StadsLens.Application.Interfaces.Repositories;
using StadsLens.Domain.Entities;
using StadsLens.Infrastructure.Data;

namespace StadsLens.Infrastructure.Repositories;

public class RouteRepository : IRouteRepository
{
    private readonly StadsLensDbContext _context;

    public RouteRepository(StadsLensDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Route>> GetAllAsync()
    {
        return await _context.Routes
            .Include(r => r.RouteLandmarks)
            .ThenInclude(rl => rl.Landmark)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Route?> GetByIdAsync(int id)
    {
        // Ook de landmarks zelf laden (zoals in GetAllAsync), anders bevat RouteResponse.Landmarks enkel null.
        return await _context.Routes
            .Include(r => r.RouteLandmarks)
            .ThenInclude(rl => rl.Landmark)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);
    }
}
