using StadsLens.Mobile.ViewModels;

namespace StadsLens.Mobile.Views;

public partial class VisitsPage : ContentPage
{
    public VisitsPage(VisitsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
