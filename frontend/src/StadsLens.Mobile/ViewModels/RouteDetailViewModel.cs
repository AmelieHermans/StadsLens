using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using StadsLens.Mobile.Constants;
using StadsLens.Mobile.Models;
using StadsLens.Mobile.Services;

namespace StadsLens.Mobile.ViewModels;

public class RouteDetailViewModel : BaseViewModel, IRouteDetailViewModel, IQueryAttributable
{
    private readonly ILandmarkDataService _landmarkDataService;
    private readonly INavigationService _navigationService;

    private Route? selectedRoute;

    public Route? SelectedRoute
    {
        get => selectedRoute;
        set => SetProperty(ref selectedRoute, value);
    }

    private ObservableCollection<Landmark> landmarks = new();

    public ObservableCollection<Landmark> Landmarks
    {
        get => landmarks;
        set => SetProperty(ref landmarks, value);
    }

    public ICommand LoadLandmarksCommand { get; set; } = null!;
    public ICommand GoToMapCommand { get; set; } = null!;
    public ICommand GoToScanCommand { get; set; } = null!;

    public RouteDetailViewModel(ILandmarkDataService landmarkDataService, INavigationService navigationService)
    {
        _landmarkDataService = landmarkDataService;
        _navigationService = navigationService;

        BindCommands();
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

    private Task GoToMapAsync() => _navigationService.NavigateToAsync(PageRoutes.Map, new Dictionary<string, object> { { nameof(Route), SelectedRoute! } });

    private Task GoToScanAsync() => _navigationService.NavigateToAsync(PageRoutes.Scan);

    private void BindCommands()
    {
        LoadLandmarksCommand = new AsyncRelayCommand(LoadLandmarksAsync);
        GoToMapCommand = new AsyncRelayCommand(GoToMapAsync);
        GoToScanCommand = new AsyncRelayCommand(GoToScanAsync);
    }
}
