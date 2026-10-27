using StadsLens.Mobile.ViewModels;

namespace StadsLens.Mobile.Views;

public partial class RouteDetailPage : ContentPage
{
    public RouteDetailPage(IRouteDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
