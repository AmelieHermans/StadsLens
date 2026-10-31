using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;
using StadsLens.Mobile.Messages;
using StadsLens.Mobile.Models;
using StadsLens.Mobile.Services;

namespace StadsLens.Mobile.ViewModels;

public class ScanViewModel : BaseViewModel, IScanViewModel
{
    // Kleiner maken voor het uploaden: Custom Vision aanvaardt maximaal 4 MB.
    private static MediaPickerOptions PhotoOptions => new()
    {
        Title = Text("Scan_PickerTitle"),
        MaximumWidth = 1600,
        MaximumHeight = 1600,
        CompressionQuality = 80,
        RotateImage = true,
        SelectionLimit = 1
    };

    private readonly ILandmarkDataService _landmarkDataService;
    private readonly IVisitDataService _visitDataService;
    private readonly IUserSessionService _userSession;
    private readonly INavigationService _navigationService;
    private readonly IMediaPicker _mediaPicker;
    private readonly ILogger<ScanViewModel> _logger;

    private string? photoPath;

    public string? PhotoPath
    {
        get => photoPath;
        set
        {
            if (SetProperty(ref photoPath, value))
                OnPropertyChanged(nameof(HasPhoto));
        }
    }

    public bool HasPhoto => !string.IsNullOrEmpty(PhotoPath);

    private string resultText = string.Empty;

    public string ResultText
    {
        get => resultText;
        set
        {
            if (SetProperty(ref resultText, value))
                OnPropertyChanged(nameof(HasResult));
        }
    }

    public bool HasResult => !string.IsNullOrEmpty(ResultText);

    private Landmark? recognizedLandmark;

    public Landmark? RecognizedLandmark
    {
        get => recognizedLandmark;
        set
        {
            if (SetProperty(ref recognizedLandmark, value))
                OnPropertyChanged(nameof(IsRecognized));
        }
    }

    public bool IsRecognized => RecognizedLandmark is not null;

    // Op een toestel zonder camera kan je enkel een bestaande foto kiezen.
    public bool IsCaptureSupported => _mediaPicker.IsCaptureSupported;

    public ICommand TakePhotoCommand { get; set; } = null!;
    public ICommand PickPhotoCommand { get; set; } = null!;
    public ICommand OpenLandmarkCommand { get; set; } = null!;
    public ICommand GoBackCommand { get; set; } = null!;

    public ScanViewModel(ILandmarkDataService landmarkDataService, IVisitDataService visitDataService, IUserSessionService userSession, INavigationService navigationService, IMediaPicker mediaPicker, ILogger<ScanViewModel> logger)
    {
        _landmarkDataService = landmarkDataService;
        _visitDataService = visitDataService;
        _userSession = userSession;
        _navigationService = navigationService;
        _mediaPicker = mediaPicker;
        _logger = logger;

        BindCommands();
    }

    private Task TakePhotoAsync() => ScanAsync(CapturePhotoAsync);

    private async Task<FileResult?> CapturePhotoAsync()
    {
#if WINDOWS
        // MAUI's Windows image processor returns a disposed stream when resize/rotation options
        // are enabled. Let Windows return the original file; upload buffering happens later.
        _logger.LogInformation("Capturing photo on Windows without MAUI image processing.");
        return await _mediaPicker.CapturePhotoAsync(new MediaPickerOptions
        {
            Title = Text("Scan_PickerTitle")
        });
#else
        return await _mediaPicker.CapturePhotoAsync(PhotoOptions);
#endif
    }

    private Task PickPhotoAsync() => ScanAsync(() => FilePicker.Default.PickAsync(new PickOptions
    {
        PickerTitle = Text("Scan_PickerTitle"),
        FileTypes = FilePickerFileType.Images
    }));

    private async Task ScanAsync(Func<Task<FileResult?>> getPhoto)
    {
        if (IsBusy)
            return;

        ErrorMessage = string.Empty;
        ResultText = string.Empty;
        RecognizedLandmark = null;

        try
        {
            _logger.LogInformation("Photo scan started.");
            var photo = await getPhoto();
            if (photo is null)
            {
                _logger.LogInformation("Photo scan cancelled: picker returned no photo.");
                return;
            }

            IsBusy = true;
            PhotoPath = photo.FullPath;
            _logger.LogInformation("Photo selected: {FileName}.", photo.FileName);

            // De backend stuurt de foto naar Azure AI Custom Vision en zoekt de landmark bij de herkende tag.
            await using var stream = await photo.OpenReadAsync();
            _logger.LogInformation("Photo stream opened; starting recognition upload.");
            var recognition = await _landmarkDataService.RecognizeAsync(stream, photo.FileName, _userSession.Language);
            _logger.LogInformation("Recognition upload completed. Recognized: {IsRecognized}.", recognition.IsRecognized);

            if (!recognition.IsRecognized || recognition.Landmark is null)
            {
                ResultText = Text("Scan_NotRecognized", recognition.Probability);
                return;
            }

            RecognizedLandmark = recognition.Landmark;
            ResultText = Text("Scan_Recognized", recognition.Landmark.Name, recognition.Probability);

            // De gebruiker staat bij de bezienswaardigheid, dus meteen als bezocht registreren.
            await RegisterVisitAsync(recognition.Landmark);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Photo scan failed.");
            ShowError(ex);
        }
        finally
        {
            IsBusy = false;
        }

        if (RecognizedLandmark is not null)
            await OpenLandmarkAsync();
    }

    private async Task RegisterVisitAsync(Landmark landmark)
    {
        if (_userSession.CurrentUser is null)
            return;

        var visits = await _visitDataService.GetByUserAsync(_userSession.CurrentUser.Id);
        if (visits.Any(visit => visit.LandmarkId == landmark.Id))
        {
            ResultText += "\n" + Text("Scan_AlreadyVisited");
            return;
        }

        var visit = await _visitDataService.CreateAsync(new Visit
        {
            UserId = _userSession.CurrentUser.Id,
            LandmarkId = landmark.Id
        });

        if (visit is not null)
        {
            ResultText += "\n" + Text("Scan_VisitAdded");
            WeakReferenceMessenger.Default.Send(new VisitRegisteredMessage(visit));
        }
    }

    private async Task OpenLandmarkAsync()
    {
        if (RecognizedLandmark is null)
            return;

        await _navigationService.NavigateToLandmarkDetailPageAsync();
        WeakReferenceMessenger.Default.Send(new LandmarkRecognizedMessage(RecognizedLandmark));
    }

    private void BindCommands()
    {
        GoBackCommand = new AsyncRelayCommand(_navigationService.NavigateBackAsync);
        TakePhotoCommand = new AsyncRelayCommand(TakePhotoAsync);
        PickPhotoCommand = new AsyncRelayCommand(PickPhotoAsync);
        OpenLandmarkCommand = new AsyncRelayCommand(OpenLandmarkAsync);
    }
}
