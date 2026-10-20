using AutoMapper;
using StadsLens.Application.Interfaces.Repositories;
using StadsLens.Application.Interfaces.Services;
using StadsLens.Contracts.Visits;
using StadsLens.Domain.Entities;

namespace StadsLens.Application.Services;

public class VisitService : IVisitService
{
    private readonly IVisitRepository _visitRepository;
    private readonly IMapper _mapper;

    public VisitService(IVisitRepository visitRepository, IMapper mapper)
    {
        _visitRepository = visitRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<VisitResponse>> GetByUserIdAsync(int userId)
    {
        var visits = await _visitRepository.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<VisitResponse>>(visits);
    }

    public async Task<VisitResponse> CreateAsync(CreateVisitRequest request)
    {
        var visit = new Visit
        {
            UserId = request.UserId,
            LandmarkId = request.LandmarkId,
            VisitedAt = DateTime.UtcNow
        };

        var created = await _visitRepository.AddAsync(visit);

        // Opnieuw ophalen zodat de naam van de landmark mee in de response zit.
        var visitWithLandmark = await _visitRepository.GetByIdAsync(created.Id);
        return _mapper.Map<VisitResponse>(visitWithLandmark ?? created);
    }
}
