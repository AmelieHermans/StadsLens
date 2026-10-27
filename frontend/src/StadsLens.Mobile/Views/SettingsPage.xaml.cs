using StadsLens.Mobile.ViewModels;

namespace StadsLens.Mobile.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(ISettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
