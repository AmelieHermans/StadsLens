using System.Globalization;
using System.Text.Json;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StadsLens.Mobile.Constants;
using StadsLens.Mobile.Messages;
using StadsLens.Mobile.Models;
using StadsLens.Mobile.Services;

namespace StadsLens.Mobile.ViewModels;

public class MapViewModel : BaseViewModel, IMapViewModel
{
    private readonly IRouteDataService _routeDataService;
    private readonly IVisitDataService _visitDataService;
    private readonly IUserSessionService _userSession;
    private readonly INavigationService _navigationService;

    private Route? selectedRoute;
    public Route? SelectedRoute { get => selectedRoute; set => SetProperty(ref selectedRoute, value); }
    private string distanceKm = "2";
    public string DistanceKm { get => distanceKm; set => SetProperty(ref distanceKm, value); }
    private string startLatitude = string.Empty;
    public string StartLatitude { get => startLatitude; set => SetProperty(ref startLatitude, value); }
    private string startLongitude = string.Empty;
    public string StartLongitude { get => startLongitude; set => SetProperty(ref startLongitude, value); }
    private WebViewSource mapSource = new HtmlWebViewSource { Html = CreateEmptyMapHtml() };
    public WebViewSource MapSource { get => mapSource; set => SetProperty(ref mapSource, value); }
    private string resultSummary = string.Empty;
    public string ResultSummary { get => resultSummary; set => SetProperty(ref resultSummary, value); }

    public string VisitedLegendText => Text("Map_VisitedLegend");
    public string NotVisitedLegendText => Text("Map_NotVisitedLegend");
    public ICommand UseCurrentLocationCommand { get; set; } = null!;
    public ICommand CreateRouteCommand { get; set; } = null!;
    public ICommand GoBackCommand { get; set; } = null!;

    public MapViewModel(IRouteDataService routeDataService, IVisitDataService visitDataService, IUserSessionService userSession, INavigationService navigationService)
    {
        _routeDataService = routeDataService;
        _visitDataService = visitDataService;
        _userSession = userSession;
        _navigationService = navigationService;
        Title = Text("Map_Title");
        BindCommands();
        Messenger.Register<MapViewModel, RouteSelectedMessage>(this, (r, m) =>
        {
            r.Receive(m);
            _ = r.LoadRouteMapAsync(m.Value);
        });
        Messenger.Register<MapViewModel, VisitRegisteredMessage>(this, (r, message) =>
        {
            if (r.SelectedRoute is not null)
                _ = r.LoadRouteMapAsync(r.SelectedRoute);
        });
    }

    public void Receive(RouteSelectedMessage message)
    {
        SelectedRoute = message.Value;
        Title = Text("Map_TitleWithRoute", message.Value.Name);
    }

    private async Task LoadRouteMapAsync(Route route)
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            // Ask for the complete route. This is the planner's largest supported distance.
            var plan = await _routeDataService.PlanRouteAsync(route.Id, 50, null, null);
            if (plan is null)
            {
                ErrorMessage = Text("Map_RouteFailed");
                return;
            }
            await ShowPlanAsync(plan);
        }
        catch (Exception ex) { ShowError(ex); }
        finally { IsBusy = false; }
    }

    private async Task UseCurrentLocationAsync()
    {
        ErrorMessage = string.Empty;
        try
        {
            var location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10)));
            if (location is null) { ErrorMessage = Text("Map_LocationUnknown"); return; }
            StartLatitude = location.Latitude.ToString(CultureInfo.InvariantCulture);
            StartLongitude = location.Longitude.ToString(CultureInfo.InvariantCulture);
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private async Task CreateRouteAsync()
    {
        ErrorMessage = string.Empty;
        if (SelectedRoute is null || !double.TryParse(DistanceKm, NumberStyles.Float, CultureInfo.InvariantCulture, out var distance) || distance <= 0)
        {
            ErrorMessage = Text("Map_InvalidDistance");
            return;
        }
        if (!TryCoordinate(StartLatitude, out var latitude) || !TryCoordinate(StartLongitude, out var longitude))
            latitude = longitude = null;

        try
        {
            IsBusy = true;
            var plan = await _routeDataService.PlanRouteAsync(SelectedRoute.Id, distance, latitude, longitude);
            if (plan is null) { ErrorMessage = Text("Map_RouteFailed"); return; }
            await ShowPlanAsync(plan);
        }
        catch (Exception ex) { ShowError(ex); }
        finally { IsBusy = false; }
    }

    private async Task ShowPlanAsync(RoutePlan plan)
    {
        var visitedIds = _userSession.CurrentUser is null
            ? new HashSet<int>()
            : (await _visitDataService.GetByUserAsync(_userSession.CurrentUser.Id)).Select(v => v.LandmarkId).ToHashSet();
        ResultSummary = $"{plan.DistanceMeters / 1000:0.0} km · {TimeSpan.FromSeconds(plan.DurationSeconds):hh\\:mm} · {plan.Landmarks.Count} landmarks" + (plan.IsEstimated ? " (schatting)" : string.Empty);
        MapSource = new HtmlWebViewSource { Html = BuildMapHtml(plan, visitedIds, Text("Map_VisitedPopup"), Text("Map_NotVisitedPopup")) };
    }

    private void BindCommands()
    {
        GoBackCommand = new AsyncRelayCommand(_navigationService.NavigateBackAsync);
        UseCurrentLocationCommand = new AsyncRelayCommand(UseCurrentLocationAsync);
        CreateRouteCommand = new AsyncRelayCommand(CreateRouteAsync);
    }

    private static bool TryCoordinate(string value, out double? result)
    {
        if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)) { result = parsed; return true; }
        result = null;
        return false;
    }

    private static string CreateEmptyMapHtml() => BuildMapShell("const map=L.map('map').setView([51.0289,4.4797],15);", string.Empty);

    private static string BuildMapHtml(RoutePlan plan, HashSet<int> visitedIds, string visitedText, string notVisitedText)
    {
        var points = JsonSerializer.Serialize(plan.Geometry.Select(x => new[] { x.Latitude, x.Longitude }));
        var markers = JsonSerializer.Serialize(plan.Landmarks.Select((x, index) => new { id = x.Id, name = x.Name, lat = x.Latitude, lon = x.Longitude, number = x.OrderInRoute > 0 ? x.OrderInRoute : index + 1, visited = visitedIds.Contains(x.Id) }));
        var visited = JsonSerializer.Serialize(visitedText);
        var notVisited = JsonSerializer.Serialize(notVisitedText);
        var setup = $"const p={points},m={markers};const map=L.map('map').setView([{plan.StartLatitude.ToString(CultureInfo.InvariantCulture)},{plan.StartLongitude.ToString(CultureInfo.InvariantCulture)}],15);";
        var overlays = $"const line=L.polyline(p,{{color:'#1464f4',weight:5}}).addTo(map);const visitedLabel={visited},notVisitedLabel={notVisited};m.forEach(x=>{{const state=x.visited?'visited':'todo';const icon=L.divIcon({{className:'',html:`<div class=\\\"route-pin ${{state}}\\\"><span>${{x.number}}</span></div>`,iconSize:[27,35],iconAnchor:[13,34]}});L.marker([x.lat,x.lon],{{icon}}).addTo(map).bindPopup(x.number+'. '+x.name+'<br>'+(x.visited?visitedLabel:notVisitedLabel));}});if(p.length)map.fitBounds(line.getBounds(),{{padding:[24,24]}});else if(m.length)map.fitBounds(m.map(x=>[x.lat,x.lon]),{{padding:[24,24]}});";
        return BuildMapShell(setup, overlays);
    }

    private static string BuildMapShell(string mapSetup, string overlays) => $$"""<!doctype html><html><head><meta name='viewport' content='width=device-width,initial-scale=1'><link rel='stylesheet' href='https://unpkg.com/leaflet@1.9.4/dist/leaflet.css'/><style>html,body,#map{height:100%;margin:0}.route-pin{position:relative;display:flex;align-items:center;justify-content:center;width:27px;height:27px;border:2px solid white;border-radius:50% 50% 50% 0;transform:rotate(-45deg);box-shadow:0 2px 5px #333;cursor:pointer}.route-pin.visited{background:#16834a}.route-pin.todo{background:#1464f4}.route-pin span{color:white;font:bold 12px sans-serif;transform:rotate(45deg)}</style></head><body><div id='map'></div><script src='https://unpkg.com/leaflet@1.9.4/dist/leaflet.js'></script><script>{{mapSetup}}L.tileLayer('{{MapSettings.CartoTileUrl}}',{attribution:'OpenStreetMap contributors CARTO'}).addTo(map);{{overlays}}</script></body></html>""";
}
