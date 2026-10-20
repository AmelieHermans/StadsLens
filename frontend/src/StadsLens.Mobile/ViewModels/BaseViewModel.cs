using CommunityToolkit.Mvvm.ComponentModel;

namespace StadsLens.Mobile.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool isBusy;

    // Apart van IsBusy: een RefreshView zet dit zelf op true vóór het command loopt.
    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private string title = string.Empty;

    public bool IsNotBusy => !IsBusy;

    // Toont een melding wanneer de API niet bereikbaar is of een fout teruggeeft.
    protected static Task ShowErrorAsync(Exception exception)
    {
        var message = exception is HttpRequestException
            ? $"Kan de server niet bereiken. Draait de API?\n\n{exception.Message}"
            : exception.Message;

        return Shell.Current.DisplayAlertAsync("Fout", message, "OK");
    }
}
