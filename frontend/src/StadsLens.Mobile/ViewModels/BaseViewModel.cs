using CommunityToolkit.Mvvm.ComponentModel;
using StadsLens.Mobile.Exceptions;
using StadsLens.Mobile.Services;

namespace StadsLens.Mobile.ViewModels;

// ObservableRecipient = ObservableObject die ook messages kan ontvangen (Messenger.Register).
public class BaseViewModel : ObservableRecipient, IBaseViewModel
{
    private string title = string.Empty;

    public string Title
    {
        get => title;
        set => SetProperty(ref title, value);
    }

    private bool isBusy;

    public bool IsBusy
    {
        get => isBusy;
        set
        {
            if (SetProperty(ref isBusy, value))
                OnPropertyChanged(nameof(IsNotBusy));
        }
    }

    public bool IsNotBusy => !IsBusy;

    // Apart van IsBusy: een RefreshView zet dit zelf op true vóór het command loopt.
    private bool isRefreshing;

    public bool IsRefreshing
    {
        get => isRefreshing;
        set => SetProperty(ref isRefreshing, value);
    }

    private string errorMessage = string.Empty;

    // De ViewModel toont zelf geen pop-ups: de View bindt op ErrorMessage (Separation of Concerns).
    public string ErrorMessage
    {
        get => errorMessage;
        set
        {
            if (SetProperty(ref errorMessage, value))
                OnPropertyChanged(nameof(HasError));
        }
    }

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    // Zet een begrijpelijke melding in de taal van de app. Teksten van .NET of van de server tonen we nooit rechtstreeks:
    // die staan in de taal van het toestel of in het Nederlands.
    protected void ShowError(Exception exception)
    {
        System.Diagnostics.Debug.WriteLine(exception);

        ErrorMessage = exception switch
        {
            LocalizedException => exception.Message,
            HttpRequestException { StatusCode: not null } http => Text("Error_ServerError", (int)http.StatusCode!),
            HttpRequestException => Text("Error_ServerUnreachable"),
            TaskCanceledException or TimeoutException => Text("Error_Timeout"),
            PermissionException => Text("Error_Permission"),
            FeatureNotSupportedException or FeatureNotEnabledException => Text("Error_FeatureUnavailable"),
            _ => Text("Error_Unexpected")
        };
    }

    // Een tekst in de taal van de app (Resources/Strings), eventueel met waarden ingevuld op {0}, {1}, ...
    protected static string Text(string key, params object[] values)
    {
        var text = LocalizationService.Instance[key];
        return values.Length == 0 ? text : string.Format(text, values);
    }
}
