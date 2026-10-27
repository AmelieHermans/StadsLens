using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StadsLens.Mobile.Constants;
using StadsLens.Mobile.Models;
using StadsLens.Mobile.Services;

namespace StadsLens.Mobile.ViewModels;

public partial class RouteDetailViewModel : BaseViewModel, IQueryAttributable
{
    private readonly ILandmarkDataService _landmarkDataService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private Route? selectedRoute;

    public ObservableCollection<Landmark> Landmarks { get; } = new();

    public RouteDetailViewModel(ILandmarkDataService landmarkDataService, INavigationService navigationService)
    {
        _landmarkDataService = landmarkDataService;
        _navigationService = navigationService;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(nameof(Route), out var value) && value is Route route)
        {
            SelectedRoute = route;
            Title = route.Name;
            LoadLandmarksCommand.Execute(null);
        }
    }

    [RelayCommand]
    private async Task LoadLandmarksAsync()
    {
        if (IsBusy || SelectedRoute is null)
            return;

        try
        {
            IsBusy = true;
            Landmarks.Clear();
            foreach (var landmark in await _landmarkDataService.GetByRouteAsync(SelectedRoute.Id))
                Landmarks.Add(landmark);
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task GoToMapAsync() => _navigationService.NavigateToAsync(PageRoutes.Map, new Dictionary<string, object> { { nameof(Route), SelectedRoute! } });

    [RelayCommand]
    private Task GoToScanAsync() => _navigationService.NavigateToAsync(PageRoutes.Scan);
}
