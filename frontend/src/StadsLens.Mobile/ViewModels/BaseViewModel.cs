using CommunityToolkit.Mvvm.ComponentModel;
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

    // Zet een begrijpelijke melding wanneer de API niet bereikbaar is of een fout teruggeeft.
    protected void ShowError(Exception exception)
    {
        ErrorMessage = exception is HttpRequestException
            ? Text("Error_ServerUnreachable", exception.Message)
            : exception.Message;
    }

    // Een tekst in de taal van de app (Resources/Strings), eventueel met waarden ingevuld op {0}, {1}, ...
    protected static string Text(string key, params object[] values)
    {
        var text = LocalizationService.Instance[key];
        return values.Length == 0 ? text : string.Format(text, values);
    }
}
