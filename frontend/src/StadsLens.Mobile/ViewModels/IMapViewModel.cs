using System.Windows.Input;
using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.ViewModels;

public interface IMapViewModel : IBaseViewModel
{
    Route? SelectedRoute { get; set; }
    string DistanceKm { get; set; }
    string StartLatitude { get; set; }
    string StartLongitude { get; set; }
    string MapHtml { get; set; }
    string ResultSummary { get; set; }
    string VisitedLegendText { get; }
    string NotVisitedLegendText { get; }

    ICommand UseCurrentLocationCommand { get; set; }
    ICommand CreateRouteCommand { get; set; }
    ICommand GoBackCommand { get; set; }
}
