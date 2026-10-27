using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StadsLens.Mobile.Messages;
using StadsLens.Mobile.Models;
using StadsLens.Mobile.Services;

namespace StadsLens.Mobile.ViewModels;

public class LoginViewModel : BaseViewModel, ILoginViewModel
{
    private readonly IUserDataService _userDataService;
    private readonly INavigationService _navigationService;
    private readonly IUserSessionService _userSession;

    private string name = string.Empty;

    public string Name
    {
        get => name;
        set => SetProperty(ref name, value);
    }

    public ICommand LoginCommand { get; set; } = null!;

    public LoginViewModel(IUserDataService userDataService, INavigationService navigationService, IUserSessionService userSession)
    {
        _userDataService = userDataService;
        _navigationService = navigationService;
        _userSession = userSession;
        Title = "StadsLens";

        BindCommands();
    }

    private async Task LoginAsync()
    {
        ErrorMessage = string.Empty;

        if (IsBusy || string.IsNullOrWhiteSpace(Name))
            return;

        try
        {
            IsBusy = true;
            User? user = await _userDataService.CreateAsync(new User { Name = Name, PreferredLanguage = "nl" });
            if (user is null)
            {
                ErrorMessage = "Gebruiker aanmaken is mislukt.";
                return;
            }

            _userSession.SetUser(user);
            WeakReferenceMessenger.Default.Send(new UserLoggedInMessage(user));
            await _navigationService.NavigateToMainPageAsync();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void BindCommands()
    {
        LoginCommand = new AsyncRelayCommand(LoginAsync);
    }
}
