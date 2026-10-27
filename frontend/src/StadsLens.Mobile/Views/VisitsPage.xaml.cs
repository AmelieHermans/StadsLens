using StadsLens.Mobile.ViewModels;

namespace StadsLens.Mobile.Views;

public partial class VisitsPage : ContentPage
{
    public VisitsPage(IVisitsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
