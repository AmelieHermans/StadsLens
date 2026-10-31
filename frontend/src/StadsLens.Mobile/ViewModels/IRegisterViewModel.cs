using System.Windows.Input;

namespace StadsLens.Mobile.ViewModels;

public interface IRegisterViewModel : IBaseViewModel
{
    string Name { get; set; }
    string Password { get; set; }
    string ConfirmPassword { get; set; }

    ICommand RegisterCommand { get; set; }
    ICommand GoBackCommand { get; set; }
}
