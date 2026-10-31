using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Text.Json;
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
    private readonly IVisitDataService _visitDataService;
    private readonly IUserSessionService _userSession;
    private readonly INavigationService _navigationService;

    private Route? selectedRoute;

    public Route? SelectedRoute
    {
        get => selectedRoute;
        set => SetProperty(ref selectedRoute, value);
    }

    private ObservableCollection<Landmark> landmarks = new();
    private RoutePlan? routePlan;
    private int? plannedRouteId;

    public ObservableCollection<Landmark> Landmarks
    {
        get => landmarks;
        set => SetProperty(ref landmarks, value);
    }

    public int VisitedLandmarkCount => Landmarks.Count(l => l.IsVisited);
    public double Progress => Landmarks.Count == 0 ? 0 : (double)VisitedLandmarkCount / Landmarks.Count;
    public string ProgressText => Text("RouteDetail_Progress", VisitedLandmarkCount, Landmarks.Count);
    public bool IsRouteComplete => Landmarks.Count > 0 && VisitedLandmarkCount == Landmarks.Count;
    public string PrimaryActionText => Text(IsRouteComplete ? "RouteDetail_WalkAgain" : VisitedLandmarkCount > 0 ? "RouteDetail_Continue" : "RouteDetail_Start");
    public WebViewSource MapPreviewSource => new HtmlWebViewSource { Html = BuildMapPreview(Landmarks) };

    public ICommand LoadLandmarksCommand { get; set; } = null!;
    public ICommand GoToMapCommand { get; set; } = null!;
    public ICommand GoToScanCommand { get; set; } = null!;
    public ICommand GoToLandmarkDetailCommand { get; set; } = null!;
    public ICommand GoBackCommand { get; set; } = null!;

    public ICommand PrimaryRouteActionCommand { get; set; } = null!;

    public RouteDetailViewModel(ILandmarkDataService landmarkDataService, IRouteDataService routeDataService, IVisitDataService visitDataService, IUserSessionService userSession, INavigationService navigationService)
    {
        _landmarkDataService = landmarkDataService;
        _routeDataService = routeDataService;
        _visitDataService = visitDataService;
        _userSession = userSession;
        _navigationService = navigationService;

        BindCommands();

        Messenger.Register<RouteDetailViewModel, RouteSelectedMessage>(this, (r, m) => r.Receive(m));
        Messenger.Register<RouteDetailViewModel, LanguageChangedMessage>(this, (r, m) => r.Receive(m));
        Messenger.Register<RouteDetailViewModel, VisitRegisteredMessage>(this, (r, _) => r.LoadLandmarksCommand.Execute(null));
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
        routePlan = null;
        plannedRouteId = null;
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
            var routeLandmarks = (await _landmarkDataService.GetByRouteAsync(SelectedRoute.Id)).ToList();
            IReadOnlyList<Visit> visits = _userSession.CurrentUser is null
                ? Array.Empty<Visit>()
                : (await _visitDataService.GetByUserAsync(_userSession.CurrentUser.Id)).ToList();
            foreach (var landmark in routeLandmarks)
            {
                landmark.VisitCount = visits.Count(v => v.LandmarkId == landmark.Id);
                landmark.IsVisited = landmark.VisitCount > 0;
                Landmarks.Add(landmark);
            }

            // Use the same road-following geometry as the full map preview.
            // If the routing endpoint is unavailable, the mini-map still falls back to connecting the landmarks.
            if (plannedRouteId != SelectedRoute.Id)
            {
                routePlan = null;
                try
                {
                    routePlan = await _routeDataService.PlanRouteAsync(SelectedRoute.Id, 50, null, null);
                }
                catch (HttpRequestException)
                {
                    // The landmark pins remain useful even if road routing is temporarily unavailable.
                }
                plannedRouteId = SelectedRoute.Id;
            }
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
        finally
        {
            IsBusy = false;
        }
        OnPropertyChanged(nameof(VisitedLandmarkCount));
        OnPropertyChanged(nameof(Progress));
        OnPropertyChanged(nameof(ProgressText));
        OnPropertyChanged(nameof(IsRouteComplete));
        OnPropertyChanged(nameof(PrimaryActionText));
        OnPropertyChanged(nameof(MapPreviewSource));
    }

    private async Task GoToMapAsync()
    {
        if (SelectedRoute is null)
            return;

        await _navigationService.NavigateToRouteMapPageAsync();
        WeakReferenceMessenger.Default.Send(new RouteSelectedMessage(SelectedRoute));
    }

    private Task GoToScanAsync() => _navigationService.NavigateToScanPageAsync();

    private Task PrimaryRouteActionAsync() => GoToMapAsync();

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
        PrimaryRouteActionCommand = new AsyncRelayCommand(PrimaryRouteActionAsync);
    }

    private string BuildMapPreview(IEnumerable<Landmark> landmarks)
    {
        var markers = landmarks.Select((l, index) => new { name = l.Name, lat = l.Latitude, lon = l.Longitude, number = l.OrderInRoute ?? index + 1, visited = l.IsVisited }).ToList();
        var markerJson = JsonSerializer.Serialize(markers);
        var routePoints = routePlan?.Geometry.Select(point => new[] { point.Latitude, point.Longitude }).ToList()
            ?? markers.Select(point => new[] { point.lat, point.lon }).ToList();
        var routeJson = JsonSerializer.Serialize(routePoints);
        var visited = JsonSerializer.Serialize(Text("Map_VisitedPopup"));
        var notVisited = JsonSerializer.Serialize(Text("Map_NotVisitedPopup"));
        return $"<!doctype html><html><head><meta name='viewport' content='width=device-width,initial-scale=1'><link rel='stylesheet' href='https://unpkg.com/leaflet@1.9.4/dist/leaflet.css'/><style>html,body,#map{{height:100%;margin:0}}.route-pin{{position:relative;display:flex;align-items:center;justify-content:center;width:27px;height:27px;border:2px solid white;border-radius:50% 50% 50% 0;transform:rotate(-45deg);box-shadow:0 2px 5px #333;cursor:pointer}}.route-pin.visited{{background:#16834a}}.route-pin.todo{{background:#1464f4}}.route-pin span{{color:white;font:bold 12px sans-serif;transform:rotate(45deg)}}</style></head><body><div id='map'></div><script src='https://unpkg.com/leaflet@1.9.4/dist/leaflet.js'></script><script>const m={markerJson},coords={routeJson},visitedLabel={visited},todoLabel={notVisited};const map=L.map('map').setView([51.0289,4.4797],14);L.tileLayer('{StadsLens.Mobile.Constants.MapSettings.CartoTileUrl}',{{attribution:'OpenStreetMap contributors'}}).addTo(map);if(coords.length>1)L.polyline(coords,{{color:'#1464f4',weight:4,opacity:.85}}).addTo(map);m.forEach(x=>{{const color=x.visited?'visited':'todo';const icon=L.divIcon({{className:'',html:`<div class=\"route-pin ${{color}}\"><span>${{x.number}}</span></div>`,iconSize:[27,35],iconAnchor:[13,34]}});L.marker([x.lat,x.lon],{{icon}}).addTo(map).bindPopup(x.number+'. '+x.name+'<br>'+(x.visited?visitedLabel:todoLabel));}});if(coords.length)map.fitBounds(coords,{{padding:[24,24]}});</script></body></html>";
    }
}
