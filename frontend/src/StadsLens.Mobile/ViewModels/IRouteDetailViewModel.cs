using System.Collections.ObjectModel;
using System.Windows.Input;
using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.ViewModels;

public interface IRouteDetailViewModel : IBaseViewModel
{
    Route? SelectedRoute { get; set; }
    ObservableCollection<Landmark> Landmarks { get; set; }

    ICommand LoadLandmarksCommand { get; set; }
    ICommand GoToMapCommand { get; set; }
    ICommand GoToScanCommand { get; set; }
}
