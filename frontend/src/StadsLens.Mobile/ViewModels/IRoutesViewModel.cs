using System.Collections.ObjectModel;
using System.Windows.Input;
using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.ViewModels;

public interface IRoutesViewModel : IBaseViewModel
{
    ObservableCollection<Route> Routes { get; set; }

    ICommand LoadRoutesCommand { get; set; }
    ICommand GoToRouteDetailCommand { get; set; }
}
