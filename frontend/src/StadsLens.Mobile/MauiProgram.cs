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
        var httpClient = new HttpClient { BaseAddress = new Uri(ApiSettings.BaseUrl) };
        // Slaat de waarschuwingspagina van gratis Ngrok-tunnels over.
        httpClient.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "true");

        services.AddSingleton(httpClient);
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IUserSessionService, UserSessionService>();
        services.AddSingleton<IRouteDataService, RouteDataService>();
        services.AddSingleton<ILandmarkDataService, LandmarkDataService>();
        services.AddSingleton<IUserDataService, UserDataService>();
        services.AddSingleton<IVisitDataService, VisitDataService>();
    }

    private static void RegisterViewModels(IServiceCollection services)
    {
        services.AddTransient<LoginViewModel>();
        services.AddTransient<RoutesViewModel>();
        services.AddTransient<RouteDetailViewModel>();
        services.AddTransient<MapViewModel>();
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
        services.AddTransient<MapPage>();
        services.AddTransient<ScanPage>();
        services.AddTransient<LandmarkDetailPage>();
        services.AddTransient<VisitsPage>();
        services.AddTransient<SettingsPage>();
    }

    private static void RegisterRoutes()
    {
        Routing.RegisterRoute(PageRoutes.RouteDetail, typeof(RouteDetailPage));
        Routing.RegisterRoute(PageRoutes.Map, typeof(MapPage));
        Routing.RegisterRoute(PageRoutes.Scan, typeof(ScanPage));
        Routing.RegisterRoute(PageRoutes.LandmarkDetail, typeof(LandmarkDetailPage));
    }
}
