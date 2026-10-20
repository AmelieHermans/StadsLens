using StadsLens.Mobile.ViewModels;

namespace StadsLens.Mobile.Views;

public partial class RoutesPage : ContentPage
{
    private readonly RoutesViewModel _viewModel;

    public RoutesPage(RoutesViewModel viewModel)
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
