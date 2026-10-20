using StadsLens.Mobile.ViewModels;

namespace StadsLens.Mobile.Views;

public partial class RouteDetailPage : ContentPage
{
    public RouteDetailPage(RouteDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
