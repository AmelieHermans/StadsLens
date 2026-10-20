using StadsLens.Mobile.ViewModels;

namespace StadsLens.Mobile.Views;

public partial class RoutesPage : ContentPage
{
    public RoutesPage(RoutesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
