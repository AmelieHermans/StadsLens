using StadsLens.Mobile.ViewModels;

namespace StadsLens.Mobile.Views;

public partial class RoutesPage : ContentPage
{
    public RoutesPage(IRoutesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
