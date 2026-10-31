namespace StadsLens.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
#if WINDOWS
        Loaded += (_, _) => ApplyNavigationCursors();
        Navigated += (_, _) => Dispatcher.Dispatch(ApplyNavigationCursors);
#endif
    }

#if WINDOWS
    private void ApplyNavigationCursors() =>
        StadsLens.Mobile.Behaviors.HandCursorBehavior.SetHandCursorOnInteractiveDescendants(Handler?.PlatformView);
#endif
}
