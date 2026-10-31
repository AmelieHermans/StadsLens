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

    private string password = string.Empty;

    public string Password
    {
        get => password;
        set => SetProperty(ref password, value);
    }

    public ICommand LoginCommand { get; set; } = null!;
    public ICommand GoToRegisterCommand { get; set; } = null!;

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

        if (IsBusy)
            return;

        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrEmpty(Password))
        {
            ErrorMessage = Text("Login_Empty");
            return;
        }

        try
        {
            IsBusy = true;
            User? user = await _userDataService.LoginAsync(Name.Trim(), Password);
            if (user is null)
            {
                ErrorMessage = Text("Login_Wrong");
                return;
            }

            // Het wachtwoord niet in het geheugen laten staan; na uitloggen is het veld weer leeg.
            Password = string.Empty;

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

    private Task GoToRegisterAsync()
    {
        ErrorMessage = string.Empty;
        return _navigationService.NavigateToRegisterPageAsync();
    }

    private void BindCommands()
    {
        LoginCommand = new AsyncRelayCommand(LoginAsync);
        GoToRegisterCommand = new AsyncRelayCommand(GoToRegisterAsync);
    }
}
