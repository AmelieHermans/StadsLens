using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StadsLens.Application.Interfaces.AI;
using StadsLens.Application.Interfaces.Repositories;
using StadsLens.Infrastructure.AI;
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

        // Azure AI-diensten (sleutels komen uit .env)
        services.AddHttpClient<ITranslationService, AzureTranslationService>();
        services.AddHttpClient<ISpeechService, AzureSpeechService>();

        return services;
    }
}
