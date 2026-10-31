using Microsoft.Maui.Controls;

#if WINDOWS
using System.Reflection;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
#endif

namespace StadsLens.Mobile.Behaviors;

/// <summary>Shows the platform hand cursor over an interactive view on Windows.</summary>
public sealed class HandCursorBehavior : Behavior<VisualElement>
{
    private VisualElement? associatedView;

    protected override void OnAttachedTo(VisualElement bindable)
    {
        base.OnAttachedTo(bindable);
        associatedView = bindable;
        bindable.HandlerChanged += OnHandlerChanged;
        bindable.PropertyChanged += OnViewPropertyChanged;
        ApplyCursor();
    }

    protected override void OnDetachingFrom(VisualElement bindable)
    {
        bindable.HandlerChanged -= OnHandlerChanged;
        bindable.PropertyChanged -= OnViewPropertyChanged;
        associatedView = null;
        base.OnDetachingFrom(bindable);
    }

    private void OnHandlerChanged(object? sender, EventArgs e) => ApplyCursor();
    private void OnViewPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(VisualElement.IsEnabled))
            ApplyCursor();
    }

    private void ApplyCursor()
    {
#if WINDOWS
        SetHandCursor(associatedView?.Handler?.PlatformView, associatedView?.IsEnabled ?? false);
#endif
    }

    /// <summary>Used by the Windows Button handler mapper and this behavior.</summary>
    public static void SetHandCursor(object? platformView, bool isEnabled = true)
    {
#if WINDOWS
        if (platformView is not FrameworkElement element)
            return;

        void SetCursor()
        {
            var property = typeof(UIElement).GetProperty("ProtectedCursor", BindingFlags.Instance | BindingFlags.NonPublic);
            property?.SetValue(element, isEnabled ? InputSystemCursor.Create(InputSystemCursorShape.Hand) : null);
        }

        RoutedEventHandler? loadedHandler = null;
        loadedHandler = (_, _) =>
        {
            element.Loaded -= loadedHandler;
            SetCursor();
        };
        element.Loaded += loadedHandler;
        if (element.IsLoaded)
        {
            element.Loaded -= loadedHandler;
            SetCursor();
        }
#endif
    }

    public static void SetHandCursorOnInteractiveDescendants(object? platformRoot)
    {
#if WINDOWS
        if (platformRoot is not DependencyObject root)
            return;

        ApplyToTree(root);

        static void ApplyToTree(DependencyObject parent)
        {
            if (parent is FrameworkElement element && IsInteractive(element))
                SetHandCursor(element);

            var count = VisualTreeHelper.GetChildrenCount(parent);
            for (var index = 0; index < count; index++)
                ApplyToTree(VisualTreeHelper.GetChild(parent, index));
        }

        static bool IsInteractive(FrameworkElement element) =>
            element is ButtonBase or NavigationViewItemBase or Microsoft.UI.Xaml.Controls.RadioButton or ToggleSwitch
            || element.GetType().Name.Contains("TabViewItem", StringComparison.Ordinal)
            || element.GetType().Name.Contains("NavigationViewItem", StringComparison.Ordinal);
#endif
    }
}
