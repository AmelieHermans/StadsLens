using StadsLens.Contracts.Visits;

namespace StadsLens.Application.Interfaces.Services;

public interface IVisitService
{
    Task<IEnumerable<VisitResponse>> GetByUserIdAsync(int userId);
    Task<VisitResponse> CreateAsync(CreateVisitRequest request);
}
