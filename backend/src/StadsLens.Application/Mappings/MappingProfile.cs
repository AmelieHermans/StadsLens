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
            .ForMember(dest => dest.LandmarkCount, opt => opt.MapFrom(src => src.Landmarks.Count));

        CreateMap<Landmark, LandmarkResponse>();

        CreateMap<User, UserResponse>();
        CreateMap<CreateUserRequest, User>();

        // LandmarkName wordt automatisch gevuld via Landmark.Name (flattening).
        CreateMap<Visit, VisitResponse>();
    }
}
