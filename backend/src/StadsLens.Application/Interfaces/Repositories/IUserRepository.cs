using StadsLens.Domain.Entities;

namespace StadsLens.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByNameAsync(string name);
    Task<User> AddAsync(User user);
}
