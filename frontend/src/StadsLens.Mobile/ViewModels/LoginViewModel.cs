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

    [ObservableProperty]
    private string name = string.Empty;

    public LoginViewModel(IStadsLensApiService apiService, INavigationService navigationService)
    {
        _apiService = apiService;
        _navigationService = navigationService;
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
                return;

            WeakReferenceMessenger.Default.Send(new UserLoggedInMessage(user));
            await _navigationService.NavigateToAsync(PageRoutes.Routes);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
