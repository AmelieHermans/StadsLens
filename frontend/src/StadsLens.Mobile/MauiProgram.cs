
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;
using StadsLens.Mobile.Constants;
using StadsLens.Mobile.Services;
using StadsLens.Mobile.ViewModels;
using StadsLens.Mobile.Views;

namespace StadsLens.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        RegisterServices(builder.Services);
        RegisterViewModels(builder.Services);
        RegisterViews(builder.Services);

        return builder.Build();
    }

    private static void RegisterServices(IServiceCollection services)
    {
        // De LanguageHandler stuurt de gekozen taal mee met elk verzoek, dus de HttpClient wordt pas
        // gemaakt wanneer de UserSessionService beschikbaar is.
        services.AddSingleton(serviceProvider =>
        {
            var httpClient = new HttpClient(new LanguageHandler(serviceProvider.GetRequiredService<IUserSessionService>()))
            {
                BaseAddress = new Uri(ApiSettings.BaseUrl)
            };

            httpClient.DefaultRequestHeaders.Add(
                "ngrok-skip-browser-warning", "true");

            return httpClient;
        });

        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<ILocalizationService>(LocalizationService.Instance);
        services.AddSingleton<IUserSessionService, UserSessionService>();
        services.AddSingleton<IRouteDataService, RouteDataService>();
        services.AddSingleton<ILandmarkDataService, LandmarkDataService>();
        services.AddSingleton<IUserDataService, UserDataService>();
        services.AddSingleton<IVisitDataService, VisitDataService>();
        services.AddSingleton(AudioManager.Current);
        services.AddSingleton(MediaPicker.Default);

        // Nieuwe service voor het genereren van wandelroutes
        services.AddTransient<IRoutingService, RoutingService>();
        services.AddTransient<ILocationSearchService, LocationSearchService>();
    }

    private static void RegisterViewModels(IServiceCollection services)
    {
        services.AddTransient<ILoginViewModel, LoginViewModel>();
        services.AddTransient<IRegisterViewModel, RegisterViewModel>();
        services.AddTransient<IRoutesViewModel, RoutesViewModel>();
        services.AddTransient<IRouteDetailViewModel, RouteDetailViewModel>();
        services.AddTransient<IMapViewModel, MapViewModel>();
        services.AddTransient<IScanViewModel, ScanViewModel>();
        services.AddTransient<ILandmarkDetailViewModel, LandmarkDetailViewModel>();
        services.AddTransient<IVisitsViewModel, VisitsViewModel>();
        services.AddTransient<ISettingsViewModel, SettingsViewModel>();
        services.AddTransient<IRouteCreateViewModel, RouteCreateViewModel>();
    }

    private static void RegisterViews(IServiceCollection services)
    {
        services.AddTransient<LoginPage>();
        services.AddTransient<RegisterPage>();
        services.AddTransient<RoutesPage>();
        services.AddTransient<RouteDetailPage>();

        // Bestaande kaartpagina
        services.AddTransient<MapPage>();
        services.AddTransient<RouteMapPage>();

        // Nieuwe pagina Route maken
        services.AddTransient<RouteCreatePage>();

        services.AddTransient<ScanPage>();
        services.AddTransient<LandmarkDetailPage>();
        services.AddTransient<VisitsPage>();
        services.AddTransient<SettingsPage>();
    }
}
