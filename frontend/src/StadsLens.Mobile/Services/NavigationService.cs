using StadsLens.Mobile.Views;

namespace StadsLens.Mobile.Services;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;

    // Shell.Current bestaat nog niet wanneer deze singleton aangemaakt wordt, dus pas ophalen bij gebruik.
    private INavigation Navigation => Shell.Current.Navigation;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task NavigateToAsync(string route, IDictionary<string, object>? parameters = null)
    {
        return parameters is null
            ? Shell.Current.GoToAsync(route)
            : Shell.Current.GoToAsync(route, parameters);
    }

    public Task GoBackAsync()
    {
        return Shell.Current.GoToAsync("..");
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
