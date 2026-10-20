using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using StadsLens.Mobile.Constants;
using StadsLens.Mobile.Models;
using StadsLens.Mobile.Services;

namespace StadsLens.Mobile.ViewModels;

public partial class RoutesViewModel : BaseViewModel
{
    private readonly IStadsLensApiService _apiService;
    private readonly INavigationService _navigationService;

    public ObservableCollection<Route> Routes { get; } = new();

    public RoutesViewModel(IStadsLensApiService apiService, INavigationService navigationService)
    {
        _apiService = apiService;
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
            foreach (var route in await _apiService.GetRoutesAsync())
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
