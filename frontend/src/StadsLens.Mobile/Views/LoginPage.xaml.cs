using StadsLens.Mobile.ViewModels;

namespace StadsLens.Mobile.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(ILoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
