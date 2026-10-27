using StadsLens.Mobile.ViewModels;

namespace StadsLens.Mobile.Views;

public partial class RoutesPage : ContentPage
{
    private readonly IRoutesViewModel _viewModel;

    public RoutesPage(IRoutesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadRoutesCommand.Execute(null);
    }
}
