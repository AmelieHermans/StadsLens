using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StadsLens.Mobile.Constants;
using StadsLens.Mobile.Messages;
using StadsLens.Mobile.Services;

namespace StadsLens.Mobile.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IStadsLensApiService _apiService;
    private readonly INavigationService _navigationService;
    private readonly IUserSessionService _userSession;

    [ObservableProperty]
    private string name = string.Empty;

    public LoginViewModel(IStadsLensApiService apiService, INavigationService navigationService, IUserSessionService userSession)
    {
        _apiService = apiService;
        _navigationService = navigationService;
        _userSession = userSession;
        Title = "StadsLens";
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy || string.IsNullOrWhiteSpace(Name))
            return;

        try
        {
            IsBusy = true;
            var user = await _apiService.CreateUserAsync(Name, "nl");
            if (user is null)
            {
                await Shell.Current.DisplayAlertAsync("Fout", "Gebruiker aanmaken is mislukt.", "OK");
                return;
            }

            _userSession.SetUser(user);
            WeakReferenceMessenger.Default.Send(new UserLoggedInMessage(user));
            await _navigationService.NavigateToAsync(PageRoutes.Routes);
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
