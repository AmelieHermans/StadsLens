using System.Collections.ObjectModel;
using System.Windows.Input;
using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.ViewModels;

public interface ITimeTravelViewModel : IBaseViewModel
{
    Landmark? Landmark { get; set; }

    // De schuifregelaar: links de oudste historische beelden, dan vandaag, helemaal rechts de toekomst.
    double SliderValue { get; set; }
    double SliderMaximum { get; }

    ObservableCollection<TimeTravelStop> Stops { get; }
    string CurrentDate { get; }
    string CurrentCaption { get; }
    string CurrentCredit { get; }
    bool HasNoHistory { get; }

    ICommand LoadImagesCommand { get; set; }
    ICommand GoBackCommand { get; set; }
}
