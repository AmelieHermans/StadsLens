using System.Windows.Input;
using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.ViewModels;

public interface IScanViewModel : IBaseViewModel
{
    string? PhotoPath { get; set; }
    bool HasPhoto { get; }
    string ResultText { get; set; }
    bool HasResult { get; }
    Landmark? RecognizedLandmark { get; set; }
    bool IsRecognized { get; }
    bool IsCaptureSupported { get; }

    ICommand TakePhotoCommand { get; set; }
    ICommand PickPhotoCommand { get; set; }
    ICommand OpenLandmarkCommand { get; set; }
    ICommand GoBackCommand { get; set; }
}
