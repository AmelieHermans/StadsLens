using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Plugin.Maui.Audio;
using StadsLens.Mobile.Messages;
using StadsLens.Mobile.Models;
using StadsLens.Mobile.Services;

namespace StadsLens.Mobile.ViewModels;

public class LandmarkDetailViewModel : BaseViewModel, ILandmarkDetailViewModel
{
    private readonly ILandmarkDataService _landmarkDataService;
    private readonly IVisitDataService _visitDataService;
    private readonly IUserSessionService _userSession;
    private readonly IAudioManager _audioManager;
    private readonly INavigationService _navigationService;

    private IAudioPlayer? _player;

    private Landmark? landmark;

    public Landmark? Landmark
    {
        get => landmark;
        set
        {
            if (SetProperty(ref landmark, value))
                OnPropertyChanged(nameof(HasImage));
        }
    }

    public bool HasImage => !string.IsNullOrWhiteSpace(Landmark?.ImageUrl);

    private bool isVisited;

    public bool IsVisited
    {
        get => isVisited;
        set
        {
            if (SetProperty(ref isVisited, value))
            {
                OnPropertyChanged(nameof(IsNotVisited));
                OnPropertyChanged(nameof(CanViewHistory));
            }
        }
    }

    public bool IsNotVisited => !IsVisited;
    public bool CanViewHistory => IsVisited;
    public int VisitCount { get; private set; }
    public string VisitCountText => Text("Landmark_VisitCount", VisitCount);

    private bool isPlaying;

    public bool IsPlaying
    {
        get => isPlaying;
        set
        {
            if (SetProperty(ref isPlaying, value))
                OnPropertyChanged(nameof(ReadAloudText));
        }
    }

    public string ReadAloudText => Text(IsPlaying ? "Landmark_Stop" : "Landmark_ReadAloud");

    private string translationWarning = string.Empty;

    public string TranslationWarning
    {
        get => translationWarning;
        set
        {
            if (SetProperty(ref translationWarning, value))
                OnPropertyChanged(nameof(HasTranslationWarning));
        }
    }

    public bool HasTranslationWarning => !string.IsNullOrEmpty(TranslationWarning);

    public ICommand ReadAloudCommand { get; set; } = null!;
    public ICommand GoBackCommand { get; set; } = null!;
    public ICommand ScanThisCommand { get; set; } = null!;

    public LandmarkDetailViewModel(ILandmarkDataService landmarkDataService, IVisitDataService visitDataService, IUserSessionService userSession, IAudioManager audioManager, INavigationService navigationService)
    {
        _landmarkDataService = landmarkDataService;
        _visitDataService = visitDataService;
        _userSession = userSession;
        _audioManager = audioManager;
        _navigationService = navigationService;
        Title = Text("Landmark_Title");

        BindCommands();

        Messenger.Register<LandmarkDetailViewModel, LandmarkSelectedMessage>(this, (r, m) => r.Receive(m));
        Messenger.Register<LandmarkDetailViewModel, LandmarkRecognizedMessage>(this, (r, m) => r.Receive(m));
        Messenger.Register<LandmarkDetailViewModel, LanguageChangedMessage>(this, (r, m) => r.Receive(m));

        newest = new WeakReference<LandmarkDetailViewModel>(this);
    }

    // Gesloten detailpagina's blijven bestaan (en messages ontvangen) tot de garbage collector ze opruimt.
    // Enkel de nieuwste ViewModel hoort bij de pagina die net geopend is, dus enkel die reageert.
    private static WeakReference<LandmarkDetailViewModel>? newest;

    private bool IsNewest => newest is not null && newest.TryGetTarget(out var viewModel) && ReferenceEquals(viewModel, this);

    // Gekozen in een lijst.
    public void Receive(LandmarkSelectedMessage message)
    {
        if (IsNewest)
            _ = LoadLandmarkAsync(message.Value, readAloud: false);
    }

    // Herkend met de camera (het bezoek is al geregistreerd door de ScanViewModel).
    // Staat "Tekst voorlezen" aan in de instellingen, dan meteen voorlezen.
    public void Receive(LandmarkRecognizedMessage message)
    {
        if (IsNewest)
            _ = LoadLandmarkAsync(message.Value, readAloud: _userSession.AutoReadAloud);
    }

    // Andere taal gekozen in de instellingen: de tekst opnieuw ophalen.
    public void Receive(LanguageChangedMessage message)
    {
        if (!IsNewest)
            return;

        OnPropertyChanged(nameof(ReadAloudText));
        if (Landmark is not null)
            _ = LoadLandmarkAsync(Landmark, readAloud: false);
    }

    private async Task LoadLandmarkAsync(Landmark selected, bool readAloud)
    {
        ErrorMessage = string.Empty;
        StopAudio();
        Landmark = selected;
        Title = selected.Name;

        try
        {
            IsBusy = true;
            TranslationWarning = string.Empty;

            // Opnieuw ophalen in de gekozen taal (Azure AI Translator in de backend).
            var translated = await _landmarkDataService.GetByIdAsync(selected.Id, _userSession.Language);
            if (translated is not null)
            {
                Landmark = translated;
                Title = translated.Name;
                if (translated.Language != _userSession.Language)
                    TranslationWarning = Text("Landmark_TranslationFailed");
            }

            if (_userSession.CurrentUser is not null)
            {
                var visits = await _visitDataService.GetByUserAsync(_userSession.CurrentUser.Id);
                IsVisited = visits.Any(visit => visit.LandmarkId == selected.Id);
                VisitCount = visits.Count(visit => visit.LandmarkId == selected.Id);
            }
            else { IsVisited = false; VisitCount = 0; }
            OnPropertyChanged(nameof(CanViewHistory));
            OnPropertyChanged(nameof(VisitCount));
            OnPropertyChanged(nameof(VisitCountText));
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
        finally
        {
            IsBusy = false;
        }

        if (readAloud && !HasError)
            await ReadAloudAsync();
    }

    private async Task ReadAloudAsync()
    {
        ErrorMessage = string.Empty;

        if (IsPlaying)
        {
            StopAudio();
            return;
        }

        if (Landmark is null || IsBusy)
            return;

        try
        {
            IsBusy = true;

            // De backend leest de tekst voor met Azure AI Speech en stuurt een mp3 terug.
            var audio = await _landmarkDataService.GetAudioAsync(Landmark.Id, _userSession.Language);
            _player = _audioManager.CreatePlayer(new MemoryStream(audio));
            _player.PlaybackEnded += OnPlaybackEnded;
            _player.Play();
            IsPlaying = true;
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

    private void OnPlaybackEnded(object? sender, EventArgs e) => MainThread.BeginInvokeOnMainThread(StopAudio);

    private void StopAudio()
    {
        // Eerst loskoppelen: Stop() kan zelf PlaybackEnded afvuren, en dat zou anders opnieuw StopAudio aanroepen.
        var player = _player;
        _player = null;
        IsPlaying = false;

        if (player is null)
            return;

        player.PlaybackEnded -= OnPlaybackEnded;
        player.Stop();
        player.Dispose();
    }

    // Eerst het voorlezen stoppen, anders praat de stem verder op de vorige pagina.
    private Task GoBackAsync()
    {
        StopAudio();
        return _navigationService.NavigateBackAsync();
    }

    private Task ScanThisAsync() => _navigationService.NavigateToScanPageAsync();

    private void BindCommands()
    {
        GoBackCommand = new AsyncRelayCommand(GoBackAsync);
        ReadAloudCommand = new AsyncRelayCommand(ReadAloudAsync);
        ScanThisCommand = new AsyncRelayCommand(ScanThisAsync);
    }
}
