using StadsLens.Mobile.ViewModels;

namespace StadsLens.Mobile.Views;

public partial class RouteMapPage : ContentPage
{
    public RouteMapPage(IMapViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
