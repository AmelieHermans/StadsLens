using System.Windows.Input;

namespace StadsLens.Mobile.ViewModels;

public interface ILoginViewModel : IBaseViewModel
{
    string Name { get; set; }

    ICommand LoginCommand { get; set; }
}
