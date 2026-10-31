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
    private readonly IVisitDataService _visitDataService;
    private readonly IUserSessionService _userSession;
    private readonly INavigationService _navigationService;
    private ObservableCollection<Route> routes = new();
    private ObservableCollection<Route> visibleRoutes = new();
    private string selectedStatus = "all";
    private string searchText = string.Empty;

    public ObservableCollection<Route> Routes
    {
        get => routes;
        set => SetProperty(ref routes, value);
    }

    public ObservableCollection<Route> VisibleRoutes
    {
        get => visibleRoutes;
        private set => SetProperty(ref visibleRoutes, value);
    }

    private Route? selectedRoute;

    public Route? SelectedRoute
    {
        get => selectedRoute;
        set => SetProperty(ref selectedRoute, value);
    }

    public bool ShowStatusFilter => Routes.Count > 4;
    public string SearchText
    {
        get => searchText;
        set { if (SetProperty(ref searchText, value)) ApplyFilter(); }
    }
    public string SelectedStatus
    {
        get => selectedStatus;
        set { if (SetProperty(ref selectedStatus, value)) ApplyFilter(); }
    }
    public string AllStatusText => Text("Routes_FilterAll");
    public string InProgressStatusText => Text("Routes_FilterInProgress");
    public string CompletedStatusText => Text("Routes_FilterCompleted");
    public ICommand LoadRoutesCommand { get; set; } = null!;
    public ICommand GoToRouteDetailCommand { get; set; } = null!;
    public ICommand SelectStatusCommand { get; set; } = null!;

    public RoutesViewModel(IRouteDataService routeDataService, IVisitDataService visitDataService, IUserSessionService userSession, INavigationService navigationService)
    {
        _routeDataService = routeDataService;
        _visitDataService = visitDataService;
        _userSession = userSession;
        _navigationService = navigationService;
        BindCommands();
        LoadRoutesCommand.Execute(null);

        Messenger.Register<RoutesViewModel, LanguageChangedMessage>(this, (r, m) => r.Receive(m));
        Messenger.Register<RoutesViewModel, VisitRegisteredMessage>(this, (r, m) => r.Receive(m));
        Messenger.Register<RoutesViewModel, UserLoggedInMessage>(this, (r, m) => r.Receive(m));
    }

    public void Receive(LanguageChangedMessage message)
    {
        OnPropertyChanged(nameof(AllStatusText));
        OnPropertyChanged(nameof(InProgressStatusText));
        OnPropertyChanged(nameof(CompletedStatusText));
        LoadRoutesCommand.Execute(null);
    }

    public void Receive(VisitRegisteredMessage message)
    {
        LoadRoutesCommand.Execute(null);
    }

    public void Receive(UserLoggedInMessage message)
    {
        LoadRoutesCommand.Execute(null);
    }

    private async Task LoadRoutesAsync()
    {
        ErrorMessage = string.Empty;
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            var loadedRoutes = (await _routeDataService.GetAllAsync()).ToList();
            var visitedIds = _userSession.CurrentUser is null
                ? new HashSet<int>()
                : (await _visitDataService.GetByUserAsync(_userSession.CurrentUser.Id)).Select(v => v.LandmarkId).ToHashSet();
            foreach (var route in loadedRoutes)
            {
                route.VisitedLandmarkCount = route.Landmarks?.Select(l => l.Id).Distinct().Count(visitedIds.Contains) ?? 0;
                route.ProgressText = Text("RouteDetail_Progress", route.VisitedLandmarkCount, route.LandmarkCount);
                route.ImageUrl ??= CoverFor(route.City);
            }
            loadedRoutes = loadedRoutes.OrderBy(r => r.Name).ToList();
            Routes = new ObservableCollection<Route>(loadedRoutes);
            OnPropertyChanged(nameof(ShowStatusFilter));
            ApplyFilter();
        }
        catch (Exception ex) { ShowError(ex); }
        finally { IsBusy = false; IsRefreshing = false; }
    }

    private void ApplyFilter()
    {
        IEnumerable<Route> filtered = Routes;
        if (SelectedStatus == "progress") filtered = filtered.Where(r => r.VisitedLandmarkCount > 0 && !r.IsComplete);
        else if (SelectedStatus == "complete") filtered = filtered.Where(r => r.IsComplete);
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var query = SearchText.Trim();
            filtered = filtered.Where(r => r.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase)
                || r.Description.Contains(query, StringComparison.CurrentCultureIgnoreCase));
        }
        VisibleRoutes = new ObservableCollection<Route>(filtered);
    }

    private void SelectStatus(string? status) { if (!string.IsNullOrWhiteSpace(status)) SelectedStatus = status; }

    private async Task GoToRouteDetailAsync()
    {
        var route = SelectedRoute;
        if (route is null)
            return;

        // Selectie meteen wissen, anders opent een tweede tik op dezelfde route niets meer.
        SelectedRoute = null;

        await _navigationService.NavigateToRouteDetailPageAsync();
        WeakReferenceMessenger.Default.Send(new RouteSelectedMessage(route));
    }

    private static string CoverFor(string city) => city.ToLowerInvariant() switch
    {
        "lier" => "https://images.unsplash.com/photo-1519608487953-e999c86e7455?auto=format&fit=crop&w=1200&q=80",
        "herentals" => "https://images.unsplash.com/photo-1511818966892-d7d671e672a2?auto=format&fit=crop&w=1200&q=80",
        "olen" => "https://images.unsplash.com/photo-1477959858617-67f85cf4f1df?auto=format&fit=crop&w=1200&q=80",
        _ => "https://images.unsplash.com/photo-1514890547357-a9ee288728e0?auto=format&fit=crop&w=1200&q=80"
    };

    private void BindCommands()
    {
        LoadRoutesCommand = new AsyncRelayCommand(LoadRoutesAsync);
        GoToRouteDetailCommand = new AsyncRelayCommand(GoToRouteDetailAsync);
        SelectStatusCommand = new RelayCommand<string>(SelectStatus);
    }
}
