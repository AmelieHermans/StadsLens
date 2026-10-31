using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StadsLens.Application.Interfaces.AI;
using StadsLens.Application.Interfaces.Repositories;
using StadsLens.Application.Interfaces.Security;
using StadsLens.Infrastructure.AI;
using StadsLens.Infrastructure.Data;
using StadsLens.Infrastructure.Repositories;
using StadsLens.Infrastructure.Security;

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

        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();

        // Azure AI-diensten (sleutels komen uit .env)
        services.AddHttpClient<ITranslationService, AzureTranslationService>();
        services.AddHttpClient<ISpeechService, AzureSpeechService>();
        services.AddHttpClient<IImageRecognitionService, AzureCustomVisionService>();

        // Een afbeelding maken duurt veel langer dan de standaardtimeout van 100 seconden toelaat.
        services.AddHttpClient<IImageGenerationService, AzureImageGenerationService>(client => client.Timeout = TimeSpan.FromMinutes(5));

        return services;
    }
}
