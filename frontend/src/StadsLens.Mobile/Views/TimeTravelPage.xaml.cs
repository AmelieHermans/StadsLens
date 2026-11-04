using StadsLens.Mobile.ViewModels;

namespace StadsLens.Mobile.Views;

public partial class TimeTravelPage : ContentPage
{
    public TimeTravelPage(ITimeTravelViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
