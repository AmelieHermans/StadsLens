using System.Globalization;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using StadsLens.Mobile.Constants;
using StadsLens.Mobile.Models;
using StadsLens.Mobile.Services;

namespace StadsLens.Mobile.ViewModels;

public partial class MapViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IRouteDataService _routeDataService;
    [ObservableProperty] private Route? selectedRoute;
    [ObservableProperty] private string distanceKm = "2";
    [ObservableProperty] private string startLatitude = string.Empty;
    [ObservableProperty] private string startLongitude = string.Empty;
    [ObservableProperty] private WebViewSource mapSource = new HtmlWebViewSource { Html = CreateEmptyMapHtml() };
    [ObservableProperty] private string resultSummary = string.Empty;

    public MapViewModel(IRouteDataService routeDataService) { _routeDataService = routeDataService; Title = "Route maken"; }
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(nameof(Route), out var value) && value is Route route) { SelectedRoute = route; Title = $"Route maken: {route.Name}"; }
    }

    [RelayCommand]
    private async Task UseCurrentLocationAsync()
    {
        try
        {
            var location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10)));
            if (location is null) { await Shell.Current.DisplayAlertAsync("Locatie", "Je huidige locatie kon niet worden bepaald.", "OK"); return; }
            StartLatitude = location.Latitude.ToString(CultureInfo.InvariantCulture); StartLongitude = location.Longitude.ToString(CultureInfo.InvariantCulture);
        }
        catch (Exception ex) { await ShowErrorAsync(ex); }
    }

    [RelayCommand]
    private async Task CreateRouteAsync()
    {
        if (SelectedRoute is null || !double.TryParse(DistanceKm, NumberStyles.Float, CultureInfo.InvariantCulture, out var distance) || distance <= 0) { await Shell.Current.DisplayAlertAsync("Route", "Geef een geldige afstand in kilometers op.", "OK"); return; }
        if (!TryCoordinate(StartLatitude, out var latitude) || !TryCoordinate(StartLongitude, out var longitude)) { latitude = longitude = null; }
        try
        {
            IsBusy = true;
            var plan = await _routeDataService.PlanRouteAsync(SelectedRoute.Id, distance, latitude, longitude);
            if (plan is null) { await Shell.Current.DisplayAlertAsync("Route", "De route kon niet worden gemaakt.", "OK"); return; }
            ResultSummary = $"{plan.DistanceMeters / 1000:0.0} km · {TimeSpan.FromSeconds(plan.DurationSeconds):hh\\:mm} · {plan.Landmarks.Count} landmarks" + (plan.IsEstimated ? " (schatting)" : string.Empty);
            MapSource = new HtmlWebViewSource { Html = BuildMapHtml(plan) };
        }
        catch (Exception ex) { await ShowErrorAsync(ex); }
        finally { IsBusy = false; }
    }

    private static bool TryCoordinate(string value, out double? result) { if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)) { result = parsed; return true; } result = null; return false; }
    private static string CreateEmptyMapHtml() => BuildMapShell("const map=L.map('map').setView([51.0289,4.4797],15);", "");
    private static string BuildMapHtml(RoutePlan plan)
    {
        var points = JsonSerializer.Serialize(plan.Geometry.Select(x => new[] { x.Latitude, x.Longitude }));
        var markers = JsonSerializer.Serialize(plan.Landmarks.Select(x => new { name = x.Name, lat = x.Latitude, lon = x.Longitude }));
        var setup = $"const p={points},m={markers};const map=L.map('map').setView([{plan.StartLatitude.ToString(CultureInfo.InvariantCulture)},{plan.StartLongitude.ToString(CultureInfo.InvariantCulture)}],15);";
        var overlays = "const line=L.polyline(p,{color:'#1464f4',weight:5}).addTo(map);L.marker(p[0]).addTo(map).bindPopup('Start');m.forEach(x=>L.marker([x.lat,x.lon]).addTo(map).bindPopup(x.name));map.fitBounds(line.getBounds(),{padding:[20,20]});";
        return BuildMapShell(setup, overlays);
    }

    private static string BuildMapShell(string mapSetup, string overlays) => $$"""<!doctype html><html><head><meta name='viewport' content='width=device-width,initial-scale=1'><link rel='stylesheet' href='https://unpkg.com/leaflet@1.9.4/dist/leaflet.css'/><style>html,body,#map{height:100%;margin:0}</style></head><body><div id='map'></div><script src='https://unpkg.com/leaflet@1.9.4/dist/leaflet.js'></script><script>{{mapSetup}}L.tileLayer('{{MapSettings.CartoTileUrl}}',{attribution:'© OpenStreetMap contributors © CARTO'}).addTo(map);{{overlays}}</script></body></html>""";
}
