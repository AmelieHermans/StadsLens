using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using StadsLens.Mobile.Constants;
using StadsLens.Mobile.Models;
using StadsLens.Mobile.Services;

namespace StadsLens.Mobile.ViewModels;

public partial class RoutesViewModel : BaseViewModel
{
    private readonly IRouteDataService _routeDataService;
    private readonly INavigationService _navigationService;

    public ObservableCollection<Route> Routes { get; } = new();

    public RoutesViewModel(IRouteDataService routeDataService, INavigationService navigationService)
    {
        _routeDataService = routeDataService;
        _navigationService = navigationService;
        Title = "Wandelroutes";
    }

    [RelayCommand]
    private async Task LoadRoutesAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            Routes.Clear();
            foreach (var route in await _routeDataService.GetAllAsync())
                Routes.Add(route);
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(ex);
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task GoToRouteDetailAsync(Route route)
    {
        await _navigationService.NavigateToAsync(PageRoutes.RouteDetail, new Dictionary<string, object>
        {
            { nameof(Route), route }
        });
    }
}
