using StadsLens.Contracts.Visits;

namespace StadsLens.Application.Interfaces.Services;

public interface IVisitService
{
    Task<IEnumerable<VisitResponse>> GetByUserIdAsync(int userId, string language = Languages.Default, CancellationToken cancellationToken = default);
    Task<VisitResponse> CreateAsync(CreateVisitRequest request);
}
