using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StadsLens.Mobile.Messages;
using StadsLens.Mobile.Models;
using StadsLens.Mobile.Services;

namespace StadsLens.Mobile.ViewModels;

public class TimeTravelViewModel : BaseViewModel, ITimeTravelViewModel
{
    private const string PastEra = "past";
    private const string FutureEra = "future";

    private readonly ILandmarkDataService _landmarkDataService;
    private readonly IUserSessionService _userSession;
    private readonly INavigationService _navigationService;

    private Landmark? landmark;

    public Landmark? Landmark
    {
        get => landmark;
        set => SetProperty(ref landmark, value);
    }

    private string? todayImage;

    public string? TodayImage
    {
        get => todayImage;
        set
        {
            if (SetProperty(ref todayImage, value))
                OnPropertyChanged(nameof(HasNoTodayImage));
        }
    }

    public bool HasNoTodayImage => TodayImage is null;

    private string? pastImage;

    public string? PastImage
    {
        get => pastImage;
        set => SetProperty(ref pastImage, value);
    }

    private string? futureImage;

    public string? FutureImage
    {
        get => futureImage;
        set => SetProperty(ref futureImage, value);
    }

    private double sliderValue;

    public double SliderValue
    {
        get => sliderValue;
        set
        {
            if (SetProperty(ref sliderValue, value))
            {
                OnPropertyChanged(nameof(PastOpacity));
                OnPropertyChanged(nameof(FutureOpacity));
                OnPropertyChanged(nameof(EraText));
            }
        }
    }

    // De beelden liggen op elkaar: hoe verder de schuifregelaar staat, hoe meer dat tijdperk doorschijnt.
    public double PastOpacity => Math.Max(0, -SliderValue);
    public double FutureOpacity => Math.Max(0, SliderValue);

    public string EraText => SliderValue switch
    {
        <= -0.5 => Text("TimeTravel_EraPast"),
        >= 0.5 => Text("TimeTravel_EraFuture"),
        _ => Text("TimeTravel_EraToday")
    };

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
        TodayImage = _userSession.GetScanPhoto(message.Value.Id) ?? message.Value.ImageUrl;
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
            var photoPath = _userSession.GetScanPhoto(Landmark.Id);
            var past = _landmarkDataService.GetTimeTravelImageAsync(Landmark.Id, PastEra, photoPath);

            // Met een scanfoto vertrekken beide beelden van die foto en kunnen ze tegelijk gemaakt worden.
            // Zonder foto vertrekt de toekomst van het verleden, anders passen de beelden niet op elkaar.
            if (photoPath is null)
            {
                await ShowWhenReadyAsync(past, path => PastImage = path);
                photoPath = PastImage;
            }

            var future = _landmarkDataService.GetTimeTravelImageAsync(Landmark.Id, FutureEra, photoPath);

            await ShowWhenReadyAsync(past, path => PastImage = path);
            await ShowWhenReadyAsync(future, path => FutureImage = path);
        }
        finally
        {
            IsBusy = false;
        }
    }

    // Mislukt één beeld, dan blijft het andere gewoon bruikbaar.
    private async Task ShowWhenReadyAsync(Task<string> image, Action<string> show)
    {
        try
        {
            show(await image);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void BindCommands()
    {
        GoBackCommand = new AsyncRelayCommand(_navigationService.NavigateBackAsync);
        LoadImagesCommand = new AsyncRelayCommand(LoadImagesAsync);
    }
}
