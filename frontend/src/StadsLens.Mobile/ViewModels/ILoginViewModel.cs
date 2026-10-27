using System.Windows.Input;

namespace StadsLens.Mobile.ViewModels;

public interface ILoginViewModel : IBaseViewModel
{
    string Name { get; set; }
    string Password { get; set; }

    ICommand LoginCommand { get; set; }
    ICommand GoToRegisterCommand { get; set; }
}
