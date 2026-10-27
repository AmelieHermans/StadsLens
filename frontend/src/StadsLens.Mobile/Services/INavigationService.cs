namespace StadsLens.Mobile.Services;

public interface INavigationService
{
    Task NavigateToAsync(string route, IDictionary<string, object>? parameters = null);
    Task GoBackAsync();

    // Navigatie volgens de les (deck 5): de pagina komt uit DI, de gegevens volgen via een message.
    Task NavigateToLandmarkDetailPageAsync();
    Task NavigateBackAsync();
}
