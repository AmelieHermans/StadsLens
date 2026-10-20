using StadsLens.Mobile.ViewModels;

namespace StadsLens.Mobile.Views;

public partial class MapPage : ContentPage
{
    public MapPage(MapViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
