using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StadsLens.Mobile.Messages;
using StadsLens.Mobile.Models;
using StadsLens.Mobile.Services;

namespace StadsLens.Mobile.ViewModels;

public class RouteCreateViewModel : BaseViewModel, IRouteCreateViewModel
{
    private static string CurrentLocationLabel => Text("Map_CurrentLocation");

    private readonly ILandmarkDataService _landmarkDataService;
    private readonly IRoutingService _routingService;
    private readonly ILocationSearchService _locationSearchService;
    private readonly IGeolocation _geolocation;
    private readonly INavigationService _navigationService;

    private CancellationTokenSource? _searchCts;

    private string startAddress = string.Empty;

    // Elke wijziging door de gebruiker start een (vertraagde) zoekopdracht naar suggesties.
    public string StartAddress
    {
        get => startAddress;
        set
        {
            if (!SetProperty(ref startAddress, value))
                return;

            // Een nieuw adres betekent dat de vorige coördinaten niet meer kloppen.
            StartLatitude = null;
            StartLongitude = null;
            _ = SearchAddressSuggestionsAsync(value?.Trim() ?? string.Empty);
        }
    }

    private string distanceKm = "5";

    public string DistanceKm
    {
        get => distanceKm;
        set => SetProperty(ref distanceKm, value);
    }

    private double? startLatitude;

    public double? StartLatitude
    {
        get => startLatitude;
        set => SetProperty(ref startLatitude, value);
    }

    private double? startLongitude;

    public double? StartLongitude
    {
        get => startLongitude;
        set => SetProperty(ref startLongitude, value);
    }

    private ObservableCollection<LocationSuggestion> locationSuggestions = new();

    public ObservableCollection<LocationSuggestion> LocationSuggestions
    {
        get => locationSuggestions;
        set => SetProperty(ref locationSuggestions, value);
    }

    private bool hasLocationSuggestions;

    public bool HasLocationSuggestions
    {
        get => hasLocationSuggestions;
        set => SetProperty(ref hasLocationSuggestions, value);
    }

    private LocationSuggestion? selectedSuggestion;

    // Een suggestie kiezen neemt die over als startlocatie.
    public LocationSuggestion? SelectedSuggestion
    {
        get => selectedSuggestion;
        set
        {
            if (!SetProperty(ref selectedSuggestion, value) || value is null)
                return;

            ApplyLocation(value.Label, value.Latitude, value.Longitude);

            // Terugzetten zodat dezelfde suggestie later opnieuw gekozen kan worden.
            selectedSuggestion = null;
            OnPropertyChanged(nameof(SelectedSuggestion));
        }
    }

    private bool useCurrentLocation;

    // Aan: de startlocatie komt van de GPS en het adresveld is uitgeschakeld. Uit: zelf een adres zoeken.
    public bool UseCurrentLocation
    {
        get => useCurrentLocation;
        set
        {
            if (!SetProperty(ref useCurrentLocation, value))
                return;

            OnPropertyChanged(nameof(IsAddressEnabled));

            if (value)
                _ = LoadCurrentLocationAsync();
            else
                ApplyLocation(string.Empty, null, null);
        }
    }

    public bool IsAddressEnabled => !UseCurrentLocation;

    private string resultSummary = Text("RouteCreate_ChooseStart");

    public string ResultSummary
    {
        get => resultSummary;
        set => SetProperty(ref resultSummary, value);
    }

    private string mapHtml = """
        <html>
        <body style="font-family:sans-serif;padding:20px">
            <p>Je gegenereerde wandelroute verschijnt hier.</p>
        </body>
        </html>
        """;

    public string MapHtml
    {
        get => mapHtml;
        set => SetProperty(ref mapHtml, value);
    }

    private ObservableCollection<Landmark> routeLandmarks = new();

    public ObservableCollection<Landmark> RouteLandmarks
    {
        get => routeLandmarks;
        set => SetProperty(ref routeLandmarks, value);
    }

    private Landmark? selectedLandmark;

    public Landmark? SelectedLandmark
    {
        get => selectedLandmark;
        set => SetProperty(ref selectedLandmark, value);
    }

    public ICommand CreateRouteCommand { get; set; } = null!;
    public ICommand GoToLandmarkDetailCommand { get; set; } = null!;

    public RouteCreateViewModel(
        ILandmarkDataService landmarkDataService,
        IRoutingService routingService,
        ILocationSearchService locationSearchService,
        INavigationService navigationService,
        IGeolocation geolocation)
    {
        _landmarkDataService = landmarkDataService;
        _routingService = routingService;
        _locationSearchService = locationSearchService;
        _geolocation = geolocation;
        _navigationService = navigationService;

        BindCommands();
    }

    // Zet adres en coördinaten zonder een nieuwe zoekopdracht te starten.
    private void ApplyLocation(string label, double? latitude, double? longitude)
    {
        _searchCts?.Cancel();
        SetProperty(ref startAddress, label, nameof(StartAddress));
        StartLatitude = latitude;
        StartLongitude = longitude;
        LocationSuggestions.Clear();
        HasLocationSuggestions = false;
    }

    private async Task LoadCurrentLocationAsync()
    {
        ErrorMessage = string.Empty;

        ApplyLocation(Text("RouteCreate_Locating"), null, null);

        try
        {
            var location = await _geolocation.GetLocationAsync(
                new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10)));

            if (location is null)
                throw new InvalidOperationException(Text("Map_LocationUnknown"));

            // De gebruiker kan de switch intussen al terug uitgezet hebben.
            if (UseCurrentLocation)
                ApplyLocation(CurrentLocationLabel, location.Latitude, location.Longitude);
        }
        catch (Exception ex)
        {
            // Zonder locatie heeft de switch geen zin: terug uitzetten zodat de gebruiker een adres kan typen.
            UseCurrentLocation = false;
            ErrorMessage = ex.Message;
        }
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

            if (query.Length < 3)
                return;

            // Even wachten zodat we niet bij elke toetsaanslag een request sturen.
            await Task.Delay(350, token);

            var results = await _locationSearchService.SearchAsync(query, token);

            if (token.IsCancellationRequested)
                return;

            foreach (var result in results)
                LocationSuggestions.Add(result);

            HasLocationSuggestions = LocationSuggestions.Count > 0;
        }
        catch (OperationCanceledException)
        {
            // Normaal wanneer de gebruiker verder typt.
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Locatie zoeken mislukt: {ex.Message}");
            LocationSuggestions.Clear();
            HasLocationSuggestions = false;
        }
    }

    private async Task CreateRouteAsync()
    {
        ErrorMessage = string.Empty;

        if (IsBusy)
            return;

        if (!double.TryParse(DistanceKm, NumberStyles.Float, CultureInfo.CurrentCulture, out var targetKm)
            && !double.TryParse(DistanceKm, NumberStyles.Float, CultureInfo.InvariantCulture, out targetKm))
        {
            ErrorMessage = Text("Map_InvalidDistance");
            return;
        }

        if (targetKm < 0.5 || targetKm > 50)
        {
            ErrorMessage = Text("RouteCreate_DistanceRange");
            return;
        }

        try
        {
            IsBusy = true;

            // Getypt maar geen suggestie gekozen: het eerste zoekresultaat nemen.
            if ((StartLatitude is null || StartLongitude is null) && !UseCurrentLocation && StartAddress.Trim().Length >= 3)
            {
                var first = (await _locationSearchService.SearchAsync(StartAddress.Trim())).FirstOrDefault();
                if (first is not null)
                    ApplyLocation(first.Label, first.Latitude, first.Longitude);
            }

            if (StartLatitude is null || StartLongitude is null)
            {
                ErrorMessage = Text(UseCurrentLocation ? "RouteCreate_LocationNotYetKnown" : "RouteCreate_ChooseSuggestion");
                return;
            }

            ResultSummary = Text("RouteCreate_Calculating");

            var landmarks = await _landmarkDataService.GetAllAsync();
            var route = await _routingService.GenerateRouteAsync(
                StartLatitude.Value,
                StartLongitude.Value,
                targetKm,
                landmarks);

            RouteLandmarks.Clear();
            foreach (var landmark in route.Landmarks)
                RouteLandmarks.Add(landmark);

            MapHtml = route.MapHtml;

            ResultSummary = Text("RouteCreate_Summary", route.DistanceKm, TimeSpan.FromMinutes(route.DurationMinutes), RouteLandmarks.Count);
        }
        catch (Exception ex)
        {
            ResultSummary = Text("Map_RouteFailed");
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    // Eerst navigeren (dan bestaat het ViewModel van de detailpagina), daarna de landmark doorsturen.
    private async Task GoToLandmarkDetailAsync()
    {
        var landmark = SelectedLandmark;
        if (landmark is null)
            return;

        // Selectie meteen wissen, anders opent een tweede tik op dezelfde landmark niets meer.
        SelectedLandmark = null;

        await _navigationService.NavigateToLandmarkDetailPageAsync();
        WeakReferenceMessenger.Default.Send(new LandmarkSelectedMessage(landmark));
    }

    private void BindCommands()
    {
        CreateRouteCommand = new AsyncRelayCommand(CreateRouteAsync);
        GoToLandmarkDetailCommand = new AsyncRelayCommand(GoToLandmarkDetailAsync);
    }
}
