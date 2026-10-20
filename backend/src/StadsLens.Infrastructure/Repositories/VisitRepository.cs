using Microsoft.EntityFrameworkCore;
using StadsLens.Application.Interfaces.Repositories;
using StadsLens.Domain.Entities;
using StadsLens.Infrastructure.Data;

namespace StadsLens.Infrastructure.Repositories;

public class VisitRepository : IVisitRepository
{
    private readonly StadsLensDbContext _context;

    public VisitRepository(StadsLensDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Visit>> GetByUserIdAsync(int userId)
    {
        return await _context.Visits
            .Include(v => v.Landmark)
            .Where(v => v.UserId == userId)
            .OrderByDescending(v => v.VisitedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Visit?> GetByIdAsync(int id)
    {
        return await _context.Visits
            .Include(v => v.Landmark)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<Visit> AddAsync(Visit visit)
    {
        _context.Visits.Add(visit);
        await _context.SaveChangesAsync();
        return visit;
    }
}
