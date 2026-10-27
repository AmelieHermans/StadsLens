using CommunityToolkit.Mvvm.Messaging;
using StadsLens.Mobile.Messages;
using StadsLens.Mobile.Models;
using StadsLens.Mobile.Services;

namespace StadsLens.Mobile.ViewModels;

public class SettingsViewModel : BaseViewModel, ISettingsViewModel
{
    private readonly IUserSessionService _userSession;

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
            WeakReferenceMessenger.Default.Send(new LanguageChangedMessage(value.Code));
        }
    }

    public SettingsViewModel(IUserSessionService userSession)
    {
        _userSession = userSession;
        Title = "Instellingen";

        selectedLanguage = Languages.FirstOrDefault(language => language.Code == _userSession.Language) ?? Languages[0];
    }
}
