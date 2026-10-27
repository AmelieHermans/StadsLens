using StadsLens.Mobile.ViewModels;

namespace StadsLens.Mobile.Views;

public partial class RouteCreatePage : ContentPage
{
    public RouteCreatePage(IRouteCreateViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
