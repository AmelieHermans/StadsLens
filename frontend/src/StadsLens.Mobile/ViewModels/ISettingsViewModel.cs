using System.Windows.Input;
using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.ViewModels;

public interface ISettingsViewModel : IBaseViewModel
{
    List<LanguageOption> Languages { get; set; }
    LanguageOption? SelectedLanguage { get; set; }
    bool AutoReadAloud { get; set; }
    string LoggedInAs { get; set; }

    ICommand LogoutCommand { get; set; }
}
