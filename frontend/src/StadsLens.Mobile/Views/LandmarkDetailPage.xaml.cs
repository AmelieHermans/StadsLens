using StadsLens.Mobile.ViewModels;

namespace StadsLens.Mobile.Views;

public partial class LandmarkDetailPage : ContentPage
{
    public LandmarkDetailPage(LandmarkDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
