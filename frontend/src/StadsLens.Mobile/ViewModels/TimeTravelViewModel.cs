using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StadsLens.Mobile.Messages;
using StadsLens.Mobile.Models;
using StadsLens.Mobile.Services;

namespace StadsLens.Mobile.ViewModels;

public class TimeTravelViewModel : BaseViewModel, ITimeTravelViewModel
{
    private readonly ILandmarkDataService _landmarkDataService;
    private readonly IUserSessionService _userSession;
    private readonly INavigationService _navigationService;

    private TimeTravelStop _today = new();
    private TimeTravelStop _future = new();
    private bool _historyLoaded;

    // Op volgorde van de schuifregelaar: historische beelden (oud naar recent), vandaag, toekomst.
    public ObservableCollection<TimeTravelStop> Stops { get; } = [];

    private Landmark? landmark;

    public Landmark? Landmark
    {
        get => landmark;
        set => SetProperty(ref landmark, value);
    }

    private double sliderValue;

    public double SliderValue
    {
        get => sliderValue;
        set
        {
            if (SetProperty(ref sliderValue, value))
                OnStopChanged();
        }
    }

    private double sliderMaximum = 1;

    public double SliderMaximum
    {
        get => sliderMaximum;
        private set => SetProperty(ref sliderMaximum, value);
    }

    // De schuifregelaar loopt vloeiend; datum en bijschrift horen bij de dichtstbijzijnde stap.
    private TimeTravelStop? CurrentStop
    {
        get
        {
            if (Stops.Count == 0)
                return null;

            return Stops[Math.Clamp((int)Math.Round(SliderValue), 0, Stops.Count - 1)];
        }
    }

    public string CurrentDate => CurrentStop?.DateText ?? string.Empty;
    public string CurrentCaption => CurrentStop?.Caption ?? string.Empty;
    public string CurrentCredit => CurrentStop?.Credit ?? string.Empty;

    // Enkel zeggen dat er geen beelden zijn als het ophalen ook echt gelukt is.
    public bool HasNoHistory => _historyLoaded && Stops.Count == 2;

    public ICommand LoadImagesCommand { get; set; } = null!;
    public ICommand GoBackCommand { get; set; } = null!;

    public TimeTravelViewModel(ILandmarkDataService landmarkDataService, IUserSessionService userSession, INavigationService navigationService)
    {
        _landmarkDataService = landmarkDataService;
        _userSession = userSession;
        _navigationService = navigationService;
        Title = Text("TimeTravel_Title");

        BindCommands();

        Messenger.Register<TimeTravelViewModel, TimeTravelRequestedMessage>(this, (r, m) => r.Receive(m));
    }

    public void Receive(TimeTravelRequestedMessage message)
    {
        // Elke pagina hoort bij één landmark. Meteen afmelden: een gesloten pagina die nog niet opgeruimd is,
        // mag bij een volgende tijdreis geen betaalde afbeeldingen meer aanvragen.
        Messenger.Unregister<TimeTravelRequestedMessage>(this);

        Landmark = message.Value;
        Title = message.Value.Name;

        var scanPhoto = _userSession.GetScanPhoto(message.Value.Id);
        _today = new TimeTravelStop
        {
            Image = scanPhoto,
            DateText = Text("TimeTravel_EraToday"),
            Credit = scanPhoto is not null ? Text("TimeTravel_YourPhoto") : message.Value.ImageCredit ?? string.Empty
        };
        _future = new TimeTravelStop
        {
            DateText = Text("TimeTravel_Future"),
            Caption = Text("TimeTravel_EraFuture")
        };

        Stops.Clear();
        Stops.Add(_today);
        Stops.Add(_future);
        SliderValue = 0;
        OnStopChanged();

        LoadImagesCommand.Execute(null);
    }

    private async Task LoadImagesAsync()
    {
        if (Landmark is null || IsBusy)
            return;

        ErrorMessage = string.Empty;

        try
        {
            IsBusy = true;

            // Mislukt één stap, dan blijven de andere gewoon bruikbaar.
            await TryAsync(LoadHistoryAsync);
            await TryAsync(LoadTodayAsync);

            // De AI vertrekt van het echte gebouw: de eigen scanfoto of anders de foto van vandaag.
            if (_future.Image is null)
                await TryAsync(LoadFutureAsync);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadHistoryAsync()
    {
        if (_historyLoaded)
            return;

        var images = (await _landmarkDataService.GetHistoricalImagesAsync(Landmark!.Id, _userSession.Language)).ToList();

        var stops = new List<TimeTravelStop>();
        foreach (var image in images)
        {
            stops.Add(new TimeTravelStop
            {
                Image = await _landmarkDataService.GetImagePathAsync(image.ImageUrl),
                DateText = image.DateLabel,
                Caption = image.Caption,
                Credit = image.Credit
            });
        }

        // De gebruiker kan intussen al geschoven hebben: op dezelfde stap blijven staan.
        var currentValue = SliderValue;
        for (var i = 0; i < stops.Count; i++)
            Stops.Insert(i, stops[i]);

        _historyLoaded = true;

        // Eerst het maximum, anders kapt de schuifregelaar de nieuwe waarde af.
        SliderMaximum = Stops.Count - 1;
        SliderValue = currentValue + stops.Count;
        OnStopChanged();
    }

    private async Task LoadTodayAsync()
    {
        if (_today.Image is null && Landmark!.ImageUrl is not null)
        {
            _today.Image = await _landmarkDataService.GetImagePathAsync(Landmark.ImageUrl);
        }
    }

    private async Task LoadFutureAsync()
    {
        _future.Image = await _landmarkDataService.GetFutureImageAsync(Landmark!.Id, _today.Image);
    }

    private async Task TryAsync(Func<Task> load)
    {
        try
        {
            await load();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void OnStopChanged()
    {
        // De beelden liggen op elkaar: tussen twee stappen vloeien ze in elkaar over.
        for (var i = 0; i < Stops.Count; i++)
            Stops[i].Opacity = Math.Max(0, 1 - Math.Abs(SliderValue - i));

        OnPropertyChanged(nameof(CurrentDate));
        OnPropertyChanged(nameof(CurrentCaption));
        OnPropertyChanged(nameof(CurrentCredit));
        OnPropertyChanged(nameof(HasNoHistory));
    }

    private void BindCommands()
    {
        GoBackCommand = new AsyncRelayCommand(_navigationService.NavigateBackAsync);
        LoadImagesCommand = new AsyncRelayCommand(LoadImagesAsync);
    }
}
