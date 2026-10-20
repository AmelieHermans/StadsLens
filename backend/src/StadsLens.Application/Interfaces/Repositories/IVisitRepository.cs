using StadsLens.Domain.Entities;

namespace StadsLens.Application.Interfaces.Repositories;

public interface IVisitRepository
{
    Task<IEnumerable<Visit>> GetByUserIdAsync(int userId);
    Task<Visit?> GetByIdAsync(int id);
    Task<Visit> AddAsync(Visit visit);
}
