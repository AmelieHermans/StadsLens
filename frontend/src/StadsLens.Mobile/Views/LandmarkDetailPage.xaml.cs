using StadsLens.Mobile.ViewModels;

namespace StadsLens.Mobile.Views;

public partial class LandmarkDetailPage : ContentPage
{
    public LandmarkDetailPage(ILandmarkDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
