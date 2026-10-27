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
                OnPropertyChanged(nameof(IsNotVisited));
        }
    }

    public bool IsNotVisited => !IsVisited;

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

    public string ReadAloudText => IsPlaying ? "Stop" : "Voorlezen";

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
    public ICommand RegisterVisitCommand { get; set; } = null!;

    public LandmarkDetailViewModel(ILandmarkDataService landmarkDataService, IVisitDataService visitDataService, IUserSessionService userSession, IAudioManager audioManager)
    {
        _landmarkDataService = landmarkDataService;
        _visitDataService = visitDataService;
        _userSession = userSession;
        _audioManager = audioManager;
        Title = "Bezienswaardigheid";

        BindCommands();

        Messenger.Register<LandmarkDetailViewModel, LandmarkSelectedMessage>(this, (r, m) => r.Receive(m));
        Messenger.Register<LandmarkDetailViewModel, LandmarkRecognizedMessage>(this, (r, m) => r.Receive(m));
        Messenger.Register<LandmarkDetailViewModel, LanguageChangedMessage>(this, (r, m) => r.Receive(m));
    }

    // Gekozen in een lijst: enkel tonen.
    public void Receive(LandmarkSelectedMessage message)
    {
        _ = LoadLandmarkAsync(message.Value, registerVisit: false);
    }

    // Herkend met de camera: de gebruiker staat er nu, dus meteen als bezocht registreren.
    public void Receive(LandmarkRecognizedMessage message)
    {
        _ = LoadLandmarkAsync(message.Value, registerVisit: true);
    }

    // Andere taal gekozen in de instellingen: de tekst opnieuw ophalen.
    public void Receive(LanguageChangedMessage message)
    {
        if (Landmark is not null)
            _ = LoadLandmarkAsync(Landmark, registerVisit: false);
    }

    private async Task LoadLandmarkAsync(Landmark selected, bool registerVisit)
    {
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
                    TranslationWarning = "Vertalen is niet gelukt, de tekst staat in het Nederlands.";
            }

            if (_userSession.CurrentUser is not null)
            {
                var visits = await _visitDataService.GetByUserAsync(_userSession.CurrentUser.Id);
                IsVisited = visits.Any(visit => visit.LandmarkId == selected.Id);
            }
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(ex);
        }
        finally
        {
            IsBusy = false;
        }

        if (registerVisit && !IsVisited)
            await RegisterVisitAsync();
    }

    private async Task ReadAloudAsync()
    {
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
            await ShowErrorAsync(ex);
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

    private async Task RegisterVisitAsync()
    {
        if (Landmark is null || _userSession.CurrentUser is null || IsVisited)
            return;

        try
        {
            var visit = await _visitDataService.CreateAsync(new Visit
            {
                UserId = _userSession.CurrentUser.Id,
                LandmarkId = Landmark.Id
            });

            if (visit is not null)
            {
                IsVisited = true;
                WeakReferenceMessenger.Default.Send(new VisitRegisteredMessage(visit));
            }
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(ex);
        }
    }

    private void BindCommands()
    {
        ReadAloudCommand = new AsyncRelayCommand(ReadAloudAsync);
        RegisterVisitCommand = new AsyncRelayCommand(RegisterVisitAsync);
    }
}
