using CommunityToolkit.Mvvm.ComponentModel;

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

    // Toont een melding wanneer de API niet bereikbaar is of een fout teruggeeft.
    protected static Task ShowErrorAsync(Exception exception)
    {
        var message = exception is HttpRequestException
            ? $"Kan de server niet bereiken. Draait de API?\n\n{exception.Message}"
            : exception.Message;

        return Shell.Current.DisplayAlertAsync("Fout", message, "OK");
    }
}
