
using StadsLens.Mobile.ViewModels;

namespace StadsLens.Mobile.Views;

public partial class RouteCreatePage : ContentPage
{
    public RouteCreatePage(RouteCreateViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void OnStartAddressTextChanged(
        object? sender,
        TextChangedEventArgs e)
    {
        if (BindingContext is not RouteCreateViewModel viewModel)
            return;

        var value = e.NewTextValue ?? string.Empty;

        if (!string.Equals(viewModel.StartAddress, value, StringComparison.Ordinal))
            viewModel.StartAddress = value;
    }
}
