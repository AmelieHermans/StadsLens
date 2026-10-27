
using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Devices.Sensors;
using StadsLens.Mobile.Models;
using StadsLens.Mobile.Services;

namespace StadsLens.Mobile.ViewModels;

public partial class RouteCreateViewModel : BaseViewModel
{
    private readonly ILandmarkDataService _landmarkDataService;
    private readonly IRoutingService _routingService;

    [ObservableProperty]
    private string startAddress = string.Empty;

    [ObservableProperty]
    private string distanceKm = "5";

    [ObservableProperty]
    private double? startLatitude;

    [ObservableProperty]
    private double? startLongitude;

    [ObservableProperty]
    private LocationSuggestion? selectedSuggestion;

    [ObservableProperty]
    private ObservableCollection<LocationSuggestion> locationSuggestions = new();

    [ObservableProperty]
    private bool hasLocationSuggestions;

    private CancellationTokenSource? _searchCts;
    private bool _settingAddressInternally;
    private readonly ILocationSearchService _locationSearchService;

    [ObservableProperty]
    private string resultSummary =
        "Kies een startlocatie en gewenste afstand.";

    [ObservableProperty]
    private HtmlWebViewSource mapSource = new()
    {
        Html = """
        <html>
        <body style="font-family:sans-serif;padding:20px">
            <p>Je gegenereerde wandelroute verschijnt hier.</p>
        </body>
        </html>
        """
    };

    public ObservableCollection<Landmark> RouteLandmarks { get; } = new();

    public RouteCreateViewModel(
        ILandmarkDataService landmarkDataService,
        IRoutingService routingService,
        ILocationSearchService locationSearchService)
    {
        _landmarkDataService = landmarkDataService;
        _routingService = routingService;
        _locationSearchService = locationSearchService;
        Title = "Route maken";
    }

    [RelayCommand]
    private async Task UseCurrentLocationAsync()
    {
        if (IsBusy)
            return;

        try
        {
            var location = await Geolocation.Default.GetLocationAsync(
                new GeolocationRequest(
                    GeolocationAccuracy.Medium,
                    TimeSpan.FromSeconds(10)));

            if (location is null)
            {
                await Shell.Current.DisplayAlertAsync(
                    "Locatie",
                    "Je huidige locatie kon niet worden bepaald.",
                    "OK");

                return;
            }

            StartLatitude = location.Latitude;
            StartLongitude = location.Longitude;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Locatiefout",
                ex.Message,
                "OK");
        }
    }

    [RelayCommand]
    private async Task CreateRouteAsync()
    {
        if (IsBusy)
            return;
        if (StartLatitude is null || StartLongitude is null)
        {
            await Shell.Current.DisplayAlertAsync(
                "Locatie ontbreekt",
                "Kies je huidige locatie of geef een startadres in.",
                "OK");

            return;
        }

        if (!double.TryParse(
                DistanceKm,
                NumberStyles.Float,
                CultureInfo.CurrentCulture,
                out var targetKm)
            && !double.TryParse(
                DistanceKm,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out targetKm))
        {
            await Shell.Current.DisplayAlertAsync(
                "Afstand",
                "Geef een geldige afstand in kilometers op.",
                "OK");
            return;
        }

        if (targetKm < 0.5 || targetKm > 50)
        {
            await Shell.Current.DisplayAlertAsync(
                "Afstand",
                "Kies een afstand tussen 0,5 en 50 km.",
                "OK");
            return;
        }

        try
        {
            IsBusy = true;
            ResultSummary = "Wandelroute berekenen...";

            if (StartLatitude is null || StartLongitude is null)
            {
                if (string.IsNullOrWhiteSpace(StartAddress) ||
                    StartAddress == "Huidige locatie")
                {
                    await Shell.Current.DisplayAlertAsync(
                        "Startlocatie",
                        "Geef een startadres in of kies je huidige locatie.",
                        "OK");
                    return;
                }

                var locations =
                    await Geocoding.Default.GetLocationsAsync(StartAddress);

                var location = locations?.FirstOrDefault();

                if (location is null)
                {
                    await Shell.Current.DisplayAlertAsync(
                        "Startlocatie",
                        "Het adres kon niet worden gevonden.",
                        "OK");
                    return;
                }

                StartLatitude = location.Latitude;
                StartLongitude = location.Longitude;
            }

            var landmarks = await _landmarkDataService.GetAllAsync();

            var route = await _routingService.GenerateRouteAsync(
                StartLatitude.Value,
                StartLongitude.Value,
                targetKm,
                landmarks);

            RouteLandmarks.Clear();

            foreach (var landmark in route.Landmarks)
                RouteLandmarks.Add(landmark);

            MapSource = new HtmlWebViewSource
            {
                Html = route.MapHtml
            };

            ResultSummary =
                $"Werkelijke wandelafstand: {route.DistanceKm:F2} km · " +
                $"Geschatte tijd: " +
                $"{TimeSpan.FromMinutes(route.DurationMinutes):hh\\:mm} · " +
                $"{RouteLandmarks.Count} bezienswaardigheden";
        }
        catch (Exception ex)
        {
            ResultSummary = "De route kon niet worden gemaakt.";

            await Shell.Current.DisplayAlertAsync("Routefout", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void SelectLocation(LocationSuggestion? suggestion)
    {
        if (suggestion is null)
            return;

        _settingAddressInternally = true;

        StartAddress = suggestion.Label;
        StartLatitude = suggestion.Latitude;
        StartLongitude = suggestion.Longitude;

        _settingAddressInternally = false;

        _searchCts?.Cancel();
        LocationSuggestions.Clear();
        HasLocationSuggestions = false;
    }
    partial void OnStartAddressChanged(string value)
    {
        if (_settingAddressInternally)
            return;

        value = value?.Trim() ?? string.Empty;

        StartLatitude = null;
        StartLongitude = null;

        _ = SearchAddressSuggestionsAsync(value);
    }

    private async Task SearchAddressSuggestionsAsync(string query)
    {
        _searchCts?.Cancel();
        _searchCts?.Dispose();
        _searchCts = new CancellationTokenSource();

        var token = _searchCts.Token;

        try
        {
            LocationSuggestions.Clear();
            HasLocationSuggestions = false;

            if (string.IsNullOrWhiteSpace(query) || query.Trim().Length < 3)
                return;

            await Task.Delay(350, token);

            var normalizedQuery = query.Trim();
            var results = await _locationSearchService.SearchAsync(
                normalizedQuery, token);

            if (token.IsCancellationRequested ||
                !string.Equals(
                    StartAddress.Trim(),
                    normalizedQuery,
                    StringComparison.OrdinalIgnoreCase))
                return;

            foreach (var result in results)
                LocationSuggestions.Add(result);

            HasLocationSuggestions = LocationSuggestions.Count > 0;
        }
        catch (OperationCanceledException)
        {
            // Normaal wanneer de gebruiker verder typt.
        }
        catch (HttpRequestException ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Locatiezoekopdracht mislukt: {ex.Message}");

            LocationSuggestions.Clear();
            HasLocationSuggestions = false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Onverwachte fout bij locatie zoeken: {ex}");

            LocationSuggestions.Clear();
            HasLocationSuggestions = false;
        }
    }

    partial void OnSelectedSuggestionChanged(
    LocationSuggestion? value)
    {
        if (value is null)
            return;

        _settingAddressInternally = true;

        StartAddress = value.Label;
        StartLatitude = value.Latitude;
        StartLongitude = value.Longitude;

        _settingAddressInternally = false;

        _searchCts?.Cancel();

        LocationSuggestions.Clear();
        HasLocationSuggestions = false;

        // Maak een nieuwe selectie mogelijk.
        SelectedSuggestion = null;
    }

}
