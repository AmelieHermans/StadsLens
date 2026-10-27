using AutoMapper;
using StadsLens.Contracts.Landmarks;
using StadsLens.Contracts.Routes;
using StadsLens.Contracts.Users;
using StadsLens.Contracts.Visits;
using StadsLens.Domain.Entities;

namespace StadsLens.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Route, RouteResponse>()
    .ForMember(
        dest => dest.LandmarkCount,
        opt => opt.MapFrom(src => src.RouteLandmarks.Count)
    )
    .ForMember(
        dest => dest.Landmarks,
        opt => opt.MapFrom(src =>
            src.RouteLandmarks
                .OrderBy(rl => rl.OrderInRoute)
                .Select(rl => rl.Landmark)
        )
    );

        // Losse landmark (bv. na een scan): geen volgorde, wel alle routes waarin hij zit.
        CreateMap<Landmark, LandmarkResponse>()
            .ForMember(dest => dest.RouteIds, opt => opt.MapFrom(src => src.RouteLandmarks.Select(rl => rl.RouteId)));

        // Landmark binnen een route: de volgorde komt van de koppeling.
        CreateMap<RouteLandmark, LandmarkResponse>()
            .IncludeMembers(src => src.Landmark);
        CreateMap<User, UserResponse>();
        CreateMap<CreateUserRequest, User>();

        // LandmarkName wordt automatisch gevuld via Landmark.Name (flattening).
        CreateMap<Visit, VisitResponse>();
    }
}
