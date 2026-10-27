using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StadsLens.Mobile.Messages;
using StadsLens.Mobile.Models;
using StadsLens.Mobile.Services;

namespace StadsLens.Mobile.ViewModels;

public class RouteDetailViewModel : BaseViewModel, IRouteDetailViewModel
{
    private readonly ILandmarkDataService _landmarkDataService;
    private readonly IRouteDataService _routeDataService;
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
    public ICommand GoToLandmarkDetailCommand { get; set; } = null!;
    public ICommand GoBackCommand { get; set; } = null!;

    public RouteDetailViewModel(ILandmarkDataService landmarkDataService, IRouteDataService routeDataService, INavigationService navigationService)
    {
        _landmarkDataService = landmarkDataService;
        _routeDataService = routeDataService;
        _navigationService = navigationService;

        BindCommands();

        Messenger.Register<RouteDetailViewModel, RouteSelectedMessage>(this, (r, m) => r.Receive(m));
        Messenger.Register<RouteDetailViewModel, LanguageChangedMessage>(this, (r, m) => r.Receive(m));
    }

    // Andere taal: de route (naam, beschrijving) en haar bezienswaardigheden vertaald opnieuw ophalen.
    public async void Receive(LanguageChangedMessage message)
    {
        if (SelectedRoute is null)
            return;

        try
        {
            var route = await _routeDataService.GetByIdAsync(SelectedRoute.Id);
            if (route is not null)
            {
                SelectedRoute = route;
                Title = route.Name;
            }
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }

        LoadLandmarksCommand.Execute(null);
    }

    public void Receive(RouteSelectedMessage message)
    {
        // Dezelfde route opnieuw (bv. wanneer de kaartpagina geopend wordt): niets te doen.
        if (SelectedRoute?.Id == message.Value.Id)
            return;

        SelectedRoute = message.Value;
        Title = message.Value.Name;
        LoadLandmarksCommand.Execute(null);
    }

    private async Task LoadLandmarksAsync()
    {
        ErrorMessage = string.Empty;

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
            ShowError(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task GoToMapAsync()
    {
        if (SelectedRoute is null)
            return;

        await _navigationService.NavigateToMapPageAsync();
        WeakReferenceMessenger.Default.Send(new RouteSelectedMessage(SelectedRoute));
    }

    private Task GoToScanAsync() => _navigationService.NavigateToScanPageAsync();

    // Eerst navigeren (dan bestaat het ViewModel van de detailpagina), daarna de landmark doorsturen.
    private async Task GoToLandmarkDetailAsync(Landmark? landmark)
    {
        if (landmark is null)
            return;

        await _navigationService.NavigateToLandmarkDetailPageAsync();
        WeakReferenceMessenger.Default.Send(new LandmarkSelectedMessage(landmark));
    }

    private void BindCommands()
    {
        GoBackCommand = new AsyncRelayCommand(_navigationService.NavigateBackAsync);
        LoadLandmarksCommand = new AsyncRelayCommand(LoadLandmarksAsync);
        GoToMapCommand = new AsyncRelayCommand(GoToMapAsync);
        GoToScanCommand = new AsyncRelayCommand(GoToScanAsync);
        GoToLandmarkDetailCommand = new AsyncRelayCommand<Landmark>(GoToLandmarkDetailAsync);
    }
}
