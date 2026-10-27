using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using StadsLens.Mobile.Models;
using StadsLens.Mobile.Services;

namespace StadsLens.Mobile.ViewModels;

public partial class VisitsViewModel : BaseViewModel
{
    private readonly IVisitDataService _visitDataService;
    private readonly IUserSessionService _userSession;

    public ObservableCollection<Visit> Visits { get; } = new();

    public VisitsViewModel(IVisitDataService visitDataService, IUserSessionService userSession)
    {
        _visitDataService = visitDataService;
        _userSession = userSession;
        Title = "Bezoeken";
    }

    [RelayCommand]
    private async Task LoadVisitsAsync()
    {
        if (IsBusy)
            return;

        if (_userSession.CurrentUser is null)
        {
            IsRefreshing = false;
            return;
        }

        try
        {
            IsBusy = true;
            Visits.Clear();
            foreach (var visit in await _visitDataService.GetByUserAsync(_userSession.CurrentUser.Id))
                Visits.Add(visit);
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
}
