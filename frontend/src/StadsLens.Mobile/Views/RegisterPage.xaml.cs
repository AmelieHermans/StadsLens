using StadsLens.Mobile.ViewModels;

namespace StadsLens.Mobile.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(IRegisterViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
