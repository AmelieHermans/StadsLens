using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StadsLens.Application.Interfaces.Repositories;
using StadsLens.Infrastructure.Data;
using StadsLens.Infrastructure.Repositories;

namespace StadsLens.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<StadsLensDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IRouteRepository, RouteRepository>();
        services.AddScoped<ILandmarkRepository, LandmarkRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IVisitRepository, VisitRepository>();

        return services;
    }
}
