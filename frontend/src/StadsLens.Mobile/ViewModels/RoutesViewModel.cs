using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StadsLens.Mobile.Messages;
using StadsLens.Mobile.Models;
using StadsLens.Mobile.Services;

namespace StadsLens.Mobile.ViewModels;

public class RoutesViewModel : BaseViewModel, IRoutesViewModel
{
    private readonly IRouteDataService _routeDataService;
    private readonly INavigationService _navigationService;

    private ObservableCollection<Route> routes = new();

    public ObservableCollection<Route> Routes
    {
        get => routes;
        set => SetProperty(ref routes, value);
    }

    public ICommand LoadRoutesCommand { get; set; } = null!;
    public ICommand GoToRouteDetailCommand { get; set; } = null!;

    public RoutesViewModel(IRouteDataService routeDataService, INavigationService navigationService)
    {
        _routeDataService = routeDataService;
        _navigationService = navigationService;
        Title = "Wandelroutes";

        BindCommands();
        LoadRoutesCommand.Execute(null);
    }

    private async Task LoadRoutesAsync()
    {
        ErrorMessage = string.Empty;

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
            ShowError(ex);
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    private async Task GoToRouteDetailAsync(Route? route)
    {
        if (route is null)
            return;

        // Eerst navigeren (dan bestaat het ViewModel van de detailpagina), daarna de route doorsturen.
        await _navigationService.NavigateToRouteDetailPageAsync();
        WeakReferenceMessenger.Default.Send(new RouteSelectedMessage(route));
    }

    private void BindCommands()
    {
        LoadRoutesCommand = new AsyncRelayCommand(LoadRoutesAsync);
        GoToRouteDetailCommand = new AsyncRelayCommand<Route>(GoToRouteDetailAsync);
    }
}
