using System.Collections.ObjectModel;
using System.Windows.Input;
using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.ViewModels;

public interface IRouteCreateViewModel : IBaseViewModel
{
    string StartAddress { get; set; }
    string DistanceKm { get; set; }
    double? StartLatitude { get; set; }
    double? StartLongitude { get; set; }

    ObservableCollection<LocationSuggestion> LocationSuggestions { get; set; }
    bool HasLocationSuggestions { get; set; }
    LocationSuggestion? SelectedSuggestion { get; set; }

    bool UseCurrentLocation { get; set; }
    bool IsAddressEnabled { get; }

    string ResultSummary { get; set; }
    string MapHtml { get; set; }
    ObservableCollection<Landmark> RouteLandmarks { get; set; }
    Landmark? SelectedLandmark { get; set; }

    ICommand CreateRouteCommand { get; set; }
    ICommand GoToLandmarkDetailCommand { get; set; }
}
