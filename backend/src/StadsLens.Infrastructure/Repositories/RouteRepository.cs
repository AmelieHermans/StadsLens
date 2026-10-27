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
        return await _context.Routes
            .Include(r => r.RouteLandmarks)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);
    }
}
