namespace StadsLens.Mobile.Services;

// Navigatie volgens de les (deck 5): één methode per pagina, de ViewModels kennen de Views niet.
// Gegevens voor de nieuwe pagina worden na het navigeren met een message verstuurd.
public interface INavigationService
{
    Task NavigateToMainPageAsync();
    Task NavigateToRouteDetailPageAsync();
    Task NavigateToMapPageAsync();
    Task NavigateToScanPageAsync();
    Task NavigateToLandmarkDetailPageAsync();
    Task NavigateBackAsync();
}
