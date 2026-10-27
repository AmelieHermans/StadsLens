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
    private readonly ContentTranslator _translator;

    public VisitService(IVisitRepository visitRepository, IMapper mapper, ContentTranslator translator)
    {
        _visitRepository = visitRepository;
        _mapper = mapper;
        _translator = translator;
    }

    public async Task<IEnumerable<VisitResponse>> GetByUserIdAsync(int userId, string language = Languages.Default, CancellationToken cancellationToken = default)
    {
        var visits = _mapper.Map<List<VisitResponse>>(await _visitRepository.GetByUserIdAsync(userId));

        // De namen van de bezochte landmarks in de taal van de gebruiker.
        var translated = await _translator.TranslateAsync(visits.Select(visit => visit.LandmarkName).ToList(), language, cancellationToken);
        if (translated is not null)
        {
            for (var i = 0; i < visits.Count; i++)
                visits[i].LandmarkName = translated[i];
        }

        return visits;
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
