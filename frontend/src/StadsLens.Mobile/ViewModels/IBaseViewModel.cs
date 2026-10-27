namespace StadsLens.Mobile.ViewModels;

// Contract met de properties die elke pagina gebruikt.
public interface IBaseViewModel
{
    string Title { get; set; }
    bool IsBusy { get; set; }
    bool IsNotBusy { get; }
    bool IsRefreshing { get; set; }

    // Foutmelding die de pagina toont (leeg = geen fout).
    string ErrorMessage { get; set; }
    bool HasError { get; }
}
