using System.Windows.Input;
using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.ViewModels;

public interface ITimeTravelViewModel : IBaseViewModel
{
    Landmark? Landmark { get; set; }
    string? TodayImage { get; set; }
    string? PastImage { get; set; }
    string? FutureImage { get; set; }
    bool HasNoTodayImage { get; }

    // De schuifregelaar: -1 = verleden, 0 = vandaag, 1 = toekomst.
    double SliderValue { get; set; }
    double PastOpacity { get; }
    double FutureOpacity { get; }
    string EraText { get; }

    ICommand LoadImagesCommand { get; set; }
    ICommand GoBackCommand { get; set; }
}
