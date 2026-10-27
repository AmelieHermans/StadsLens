using StadsLens.Mobile.ViewModels;

namespace StadsLens.Mobile.Views;

public partial class ScanPage : ContentPage
{
    public ScanPage(IScanViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
