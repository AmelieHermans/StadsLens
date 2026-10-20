using AutoMapper;
using StadsLens.Application.Interfaces.Repositories;
using StadsLens.Application.Interfaces.Services;
using StadsLens.Contracts.Landmarks;

namespace StadsLens.Application.Services;

public class LandmarkService : ILandmarkService
{
    private readonly ILandmarkRepository _landmarkRepository;
    private readonly IMapper _mapper;

    public LandmarkService(ILandmarkRepository landmarkRepository, IMapper mapper)
    {
        _landmarkRepository = landmarkRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LandmarkResponse>> GetByRouteIdAsync(int routeId)
    {
        var landmarks = await _landmarkRepository.GetByRouteIdAsync(routeId);
        return _mapper.Map<IEnumerable<LandmarkResponse>>(landmarks);
    }

    public async Task<LandmarkResponse?> GetByIdAsync(int id)
    {
        var landmark = await _landmarkRepository.GetByIdAsync(id);
        return landmark is null ? null : _mapper.Map<LandmarkResponse>(landmark);
    }

    public async Task<LandmarkResponse?> GetByCustomVisionTagAsync(string tag)
    {
        var landmark = await _landmarkRepository.GetByCustomVisionTagAsync(tag);
        return landmark is null ? null : _mapper.Map<LandmarkResponse>(landmark);
    }
}
