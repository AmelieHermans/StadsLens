using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StadsLens.Mobile.Messages;
using StadsLens.Mobile.Models;
using StadsLens.Mobile.Services;

namespace StadsLens.Mobile.ViewModels;

public class SettingsViewModel : BaseViewModel, ISettingsViewModel
{
    private readonly IUserSessionService _userSession;
    private readonly INavigationService _navigationService;

    private List<LanguageOption> languages = LanguageOption.All;

    public List<LanguageOption> Languages
    {
        get => languages;
        set => SetProperty(ref languages, value);
    }

    private LanguageOption? selectedLanguage;

    // Een andere taal kiezen bewaart die en laat de rest van de app het weten via een message.
    public LanguageOption? SelectedLanguage
    {
        get => selectedLanguage;
        set
        {
            if (!SetProperty(ref selectedLanguage, value) || value is null || value.Code == _userSession.Language)
                return;

            _userSession.SetLanguage(value.Code);
            UpdateLoggedInAs();
            WeakReferenceMessenger.Default.Send(new LanguageChangedMessage(value.Code));
        }
    }

    private bool autoReadAloud;

    // Aan: na een scan wordt de geschiedenis meteen voorgelezen. Uit: de gebruiker leest zelf.
    public bool AutoReadAloud
    {
        get => autoReadAloud;
        set
        {
            if (SetProperty(ref autoReadAloud, value))
                _userSession.SetAutoReadAloud(value);
        }
    }

    private string loggedInAs = string.Empty;

    public string LoggedInAs
    {
        get => loggedInAs;
        set => SetProperty(ref loggedInAs, value);
    }

    public ICommand LogoutCommand { get; set; } = null!;

    public SettingsViewModel(IUserSessionService userSession, INavigationService navigationService)
    {
        _userSession = userSession;
        _navigationService = navigationService;

        selectedLanguage = Languages.FirstOrDefault(language => language.Code == _userSession.Language) ?? Languages[0];
        autoReadAloud = _userSession.AutoReadAloud;
        UpdateLoggedInAs();

        BindCommands();

        // De tabbladen blijven bestaan na uitloggen: bij een nieuwe login de naam bijwerken.
        Messenger.Register<SettingsViewModel, UserLoggedInMessage>(this, (r, m) => r.Receive(m));
    }

    public void Receive(UserLoggedInMessage message)
    {
        UpdateLoggedInAs();
    }

    private void UpdateLoggedInAs()
    {
        LoggedInAs = Text("Settings_LoggedInAs", _userSession.CurrentUser?.Name ?? string.Empty);
    }

    private async Task LogoutAsync()
    {
        _userSession.Clear();
        await _navigationService.NavigateToLoginPageAsync();
    }

    private void BindCommands()
    {
        LogoutCommand = new AsyncRelayCommand(LogoutAsync);
    }
}
