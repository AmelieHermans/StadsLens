using Microsoft.EntityFrameworkCore;
using StadsLens.Application.Interfaces.Repositories;
using StadsLens.Domain.Entities;
using StadsLens.Infrastructure.Data;

namespace StadsLens.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly StadsLensDbContext _context;

    public UserRepository(StadsLensDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    // SQL Server vergelijkt standaard hoofdletterongevoelig: "Kayn" en "kayn" zijn dezelfde naam.
    public async Task<User?> GetByNameAsync(string name)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Name == name);
    }

    public async Task<User> AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
}
