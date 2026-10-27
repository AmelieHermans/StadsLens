
using Microsoft.Extensions.Logging;
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
        RegisterRoutes();

        return builder.Build();
    }

    private static void RegisterServices(IServiceCollection services)
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(ApiSettings.BaseUrl)
        };

        httpClient.DefaultRequestHeaders.Add(
            "ngrok-skip-browser-warning", "true");

        services.AddSingleton(httpClient);

        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IUserSessionService, UserSessionService>();
        services.AddSingleton<IRouteDataService, RouteDataService>();
        services.AddSingleton<ILandmarkDataService, LandmarkDataService>();
        services.AddSingleton<IUserDataService, UserDataService>();
        services.AddSingleton<IVisitDataService, VisitDataService>();

        // Nieuwe service voor het genereren van wandelroutes
        services.AddTransient<IRoutingService, RoutingService>();
        services.AddTransient<ILocationSearchService, LocationSearchService>();
    }

    private static void RegisterViewModels(IServiceCollection services)
    {
        services.AddTransient<LoginViewModel>();
        services.AddTransient<RoutesViewModel>();
        services.AddTransient<RouteDetailViewModel>();

        // Bestaande pagina's
        services.AddTransient<MapViewModel>();

        // Nieuwe pagina Route maken
        services.AddTransient<RouteCreateViewModel>();

        services.AddTransient<ScanViewModel>();
        services.AddTransient<LandmarkDetailViewModel>();
        services.AddTransient<VisitsViewModel>();
        services.AddTransient<SettingsViewModel>();
    }

    private static void RegisterViews(IServiceCollection services)
    {
        services.AddTransient<LoginPage>();
        services.AddTransient<RoutesPage>();
        services.AddTransient<RouteDetailPage>();

        // Bestaande kaartpagina
        services.AddTransient<MapPage>();

        // Nieuwe pagina Route maken
        services.AddTransient<RouteCreatePage>();

        services.AddTransient<ScanPage>();
        services.AddTransient<LandmarkDetailPage>();
        services.AddTransient<VisitsPage>();
        services.AddTransient<SettingsPage>();
    }

    private static void RegisterRoutes()
    {
        Routing.RegisterRoute(
            PageRoutes.RouteDetail,
            typeof(RouteDetailPage));

        Routing.RegisterRoute(
            PageRoutes.Map,
            typeof(MapPage));

        // Nieuwe navigatieroute
        Routing.RegisterRoute(
            PageRoutes.RouteCreate,
            typeof(RouteCreatePage));

        Routing.RegisterRoute(
            PageRoutes.Scan,
            typeof(ScanPage));

        Routing.RegisterRoute(
            PageRoutes.LandmarkDetail,
            typeof(LandmarkDetailPage));
    }
}
