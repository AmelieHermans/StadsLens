using StadsLens.Mobile.ViewModels;

namespace StadsLens.Mobile.Views;

public partial class VisitsPage : ContentPage
{
    private readonly VisitsViewModel _viewModel;

    public VisitsPage(VisitsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadVisitsCommand.Execute(null);
    }
}
