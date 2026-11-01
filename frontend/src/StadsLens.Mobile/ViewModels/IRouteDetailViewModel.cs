using System.Collections.ObjectModel;
using System.Windows.Input;
using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.ViewModels;

public interface IRouteDetailViewModel : IBaseViewModel
{
    Route? SelectedRoute { get; set; }
    ObservableCollection<Landmark> Landmarks { get; set; }
    Landmark? SelectedLandmark { get; set; }
    int VisitedLandmarkCount { get; }
    double Progress { get; }
    string ProgressText { get; }
    string PrimaryActionText { get; }
    bool IsRouteComplete { get; }
    string MapPreviewHtml { get; }

    ICommand LoadLandmarksCommand { get; set; }
    ICommand GoToMapCommand { get; set; }
    ICommand GoToScanCommand { get; set; }
    ICommand GoToLandmarkDetailCommand { get; set; }
    ICommand GoBackCommand { get; set; }
    ICommand PrimaryRouteActionCommand { get; set; }
}
