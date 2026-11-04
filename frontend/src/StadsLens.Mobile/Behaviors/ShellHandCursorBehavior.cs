namespace StadsLens.Mobile.Behaviors;

/// <summary>Shows the hand cursor over the tabs and navigation buttons of the Shell on Windows.</summary>
public sealed class ShellHandCursorBehavior : Behavior<Shell>
{
    protected override void OnAttachedTo(Shell bindable)
    {
        base.OnAttachedTo(bindable);
        bindable.Loaded += OnLoaded;
        bindable.Navigated += OnNavigated;
    }

    protected override void OnDetachingFrom(Shell bindable)
    {
        bindable.Loaded -= OnLoaded;
        bindable.Navigated -= OnNavigated;
        base.OnDetachingFrom(bindable);
    }

    private void OnLoaded(object? sender, EventArgs e) => Apply(sender as Shell);

    // Na een navigatie bestaan de nieuwe knoppen pas een tik later.
    private void OnNavigated(object? sender, ShellNavigatedEventArgs e)
    {
        if (sender is Shell shell)
            shell.Dispatcher.Dispatch(() => Apply(shell));
    }

    private static void Apply(Shell? shell) =>
        HandCursorBehavior.SetHandCursorOnInteractiveDescendants(shell?.Handler?.PlatformView);
}
