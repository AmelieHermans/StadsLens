using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StadsLens.Mobile.Messages;
using StadsLens.Mobile.Services;

namespace StadsLens.Mobile.ViewModels;

public class RegisterViewModel : BaseViewModel, IRegisterViewModel
{
    // Dezelfde regels als in de backend (RegisterRequest).
    private const int MinimumNameLength = 2;
    private const int MinimumPasswordLength = 6;

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

    private string confirmPassword = string.Empty;

    public string ConfirmPassword
    {
        get => confirmPassword;
        set => SetProperty(ref confirmPassword, value);
    }

    public ICommand RegisterCommand { get; set; } = null!;
    public ICommand GoBackCommand { get; set; } = null!;

    public RegisterViewModel(IUserDataService userDataService, INavigationService navigationService, IUserSessionService userSession)
    {
        _userDataService = userDataService;
        _navigationService = navigationService;
        _userSession = userSession;

        BindCommands();
    }

    private async Task RegisterAsync()
    {
        ErrorMessage = string.Empty;

        if (IsBusy)
            return;

        if (Name.Trim().Length < MinimumNameLength)
        {
            ErrorMessage = Text("Register_NameTooShort", MinimumNameLength);
            return;
        }

        if (Password.Length < MinimumPasswordLength)
        {
            ErrorMessage = Text("Register_PasswordTooShort", MinimumPasswordLength);
            return;
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = Text("Register_PasswordMismatch");
            return;
        }

        try
        {
            IsBusy = true;
            var user = await _userDataService.RegisterAsync(Name.Trim(), Password, _userSession.Language);
            if (user is null)
            {
                ErrorMessage = Text("Register_NameTaken", Name.Trim());
                return;
            }

            Password = string.Empty;
            ConfirmPassword = string.Empty;

            // Na het aanmaken meteen ingelogd.
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
        GoBackCommand = new AsyncRelayCommand(_navigationService.NavigateBackAsync);
        RegisterCommand = new AsyncRelayCommand(RegisterAsync);
    }
}
