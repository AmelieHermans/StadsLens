using StadsLens.Mobile.Views;

namespace StadsLens.Mobile.Services;

public class NavigationService : INavigationService
{
    // De TabBar uit AppShell.xaml (Routes, Bezoeken, ...).
    private const string MainTabsRoute = "//main/routes";

    private readonly IServiceProvider _serviceProvider;

    // Shell.Current bestaat nog niet wanneer deze singleton aangemaakt wordt, dus pas ophalen bij gebruik.
    private INavigation Navigation => Shell.Current.Navigation;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    // Van het loginscherm naar de tabbladen: dat is een wissel binnen de Shell, geen pagina op de stack.
    public async Task NavigateToMainPageAsync()
    {
        await Shell.Current.GoToAsync(MainTabsRoute);
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
