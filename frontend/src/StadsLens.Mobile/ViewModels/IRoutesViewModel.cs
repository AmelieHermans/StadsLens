using System.Collections.ObjectModel;
using System.Windows.Input;
using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.ViewModels;

public interface IRoutesViewModel : IBaseViewModel
{
    ObservableCollection<Route> Routes { get; set; }
    ObservableCollection<Route> VisibleRoutes { get; }
    Route? SelectedRoute { get; set; }
    bool ShowStatusFilter { get; }
    string SearchText { get; set; }
    string SelectedStatus { get; set; }
    string AllStatusText { get; }
    string InProgressStatusText { get; }
    string CompletedStatusText { get; }

    ICommand LoadRoutesCommand { get; set; }
    ICommand GoToRouteDetailCommand { get; set; }
    ICommand SelectStatusCommand { get; set; }
}
