using System.Collections.ObjectModel;
using System.Windows.Input;
using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.ViewModels;

public interface IVisitsViewModel : IBaseViewModel
{
    ObservableCollection<Visit> Visits { get; set; }

    ICommand LoadVisitsCommand { get; set; }
}
