using Microsoft.Extensions.DependencyInjection;
using StadsLens.Application.Interfaces.Services;
using StadsLens.Application.Mappings;
using StadsLens.Application.Services;

namespace StadsLens.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, typeof(MappingProfile));

        services.AddScoped<HttpClient>();
        services.AddScoped<ContentTranslator>();
        services.AddScoped<IRouteService, RouteService>();
        services.AddScoped<ILandmarkService, LandmarkService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IVisitService, VisitService>();

        return services;
    }
}
