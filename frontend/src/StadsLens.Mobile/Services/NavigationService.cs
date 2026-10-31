using StadsLens.Mobile.Views;

namespace StadsLens.Mobile.Services;

public class NavigationService : INavigationService
{
    // Routes uit AppShell.xaml: het loginscherm en de TabBar (Routes, Bezoeken, ...).
    private const string LoginRoute = "//login";
    private const string MainTabsRoute = "//main/routes";

    private readonly IServiceProvider _serviceProvider;

    // Shell.Current bestaat nog niet wanneer deze singleton aangemaakt wordt, dus pas ophalen bij gebruik.
    private INavigation Navigation => Shell.Current.Navigation;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    // Tussen het loginscherm en de tabbladen wisselen gebeurt binnen de Shell, niet met een pagina op de stack.
    public async Task NavigateToLoginPageAsync()
    {
        await Shell.Current.GoToAsync(LoginRoute);
    }

    public async Task NavigateToMainPageAsync()
    {
        await Shell.Current.GoToAsync(MainTabsRoute);
    }

    public async Task NavigateToRegisterPageAsync()
    {
        await Navigation.PushAsync(_serviceProvider.GetRequiredService<RegisterPage>());
    }

    // De pagina's komen uit de serviceprovider, zodat ze hun ViewModel via DI krijgen.
    public async Task NavigateToRouteDetailPageAsync()
    {
        await Navigation.PushAsync(_serviceProvider.GetRequiredService<RouteDetailPage>());
    }

    public async Task NavigateToMapPageAsync()
    {
        await Navigation.PushAsync(_serviceProvider.GetRequiredService<MapPage>());
    }

    public async Task NavigateToRouteMapPageAsync()
    {
        await Navigation.PushAsync(_serviceProvider.GetRequiredService<RouteMapPage>());
    }

    public async Task NavigateToScanPageAsync()
    {
        await Navigation.PushAsync(_serviceProvider.GetRequiredService<ScanPage>());
    }

    public async Task NavigateToLandmarkDetailPageAsync()
    {
        await Navigation.PushAsync(_serviceProvider.GetRequiredService<LandmarkDetailPage>());
    }

    public async Task NavigateBackAsync()
    {
        if (Navigation.NavigationStack.Count > 1)
        {
            await Navigation.PopAsync();
        }
        else
        {
            throw new InvalidOperationException("No pages to navigate back to!");
        }
    }
}
