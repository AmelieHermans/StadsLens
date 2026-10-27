using System.Windows.Input;
using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.ViewModels;

public interface ILandmarkDetailViewModel : IBaseViewModel
{
    Landmark? Landmark { get; set; }
    bool HasImage { get; }
    bool IsVisited { get; set; }
    bool IsNotVisited { get; }
    bool IsPlaying { get; set; }
    string ReadAloudText { get; }
    string TranslationWarning { get; set; }
    bool HasTranslationWarning { get; }

    ICommand ReadAloudCommand { get; set; }
    ICommand RegisterVisitCommand { get; set; }
}
