using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StadsLens.Mobile.Messages;
using StadsLens.Mobile.Models;
using StadsLens.Mobile.Services;

namespace StadsLens.Mobile.ViewModels;

public class VisitsViewModel : BaseViewModel, IVisitsViewModel
{
    private readonly IVisitDataService _visitDataService;
    private readonly IUserSessionService _userSession;

    private ObservableCollection<Visit> visits = new();

    public ObservableCollection<Visit> Visits
    {
        get => visits;
        set => SetProperty(ref visits, value);
    }

    public ICommand LoadVisitsCommand { get; set; } = null!;

    public VisitsViewModel(IVisitDataService visitDataService, IUserSessionService userSession)
    {
        _visitDataService = visitDataService;
        _userSession = userSession;
        Title = "Bezoeken";

        BindCommands();

        Messenger.Register<VisitsViewModel, VisitRegisteredMessage>(this, (r, m) => r.Receive(m));
    }

    // Een nieuw bezoek (bv. vanop de detailpagina): de lijst opnieuw laden.
    public void Receive(VisitRegisteredMessage message)
    {
        LoadVisitsCommand.Execute(null);
    }

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

    private void BindCommands()
    {
        LoadVisitsCommand = new AsyncRelayCommand(LoadVisitsAsync);
    }
}
