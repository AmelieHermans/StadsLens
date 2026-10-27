using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.ViewModels;

public interface ISettingsViewModel : IBaseViewModel
{
    List<LanguageOption> Languages { get; set; }
    LanguageOption? SelectedLanguage { get; set; }
}
