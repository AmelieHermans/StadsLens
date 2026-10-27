using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.Services;

public interface IVisitDataService : IApiService<Visit>
{
    Task<IEnumerable<Visit>> GetByUserAsync(int userId);
}
