using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GenFreeBrowser.ViewModels.Interfaces;

namespace GenFreeBrowser.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private object? currentViewModel;

    private readonly IPersonenListViewModel _personenListViewModel;

    public MainWindowViewModel(IPersonenListViewModel personenListViewModel)
    {
        _personenListViewModel = personenListViewModel;
        CurrentViewModel = _personenListViewModel;
        // initial laden (fire and forget)
        _ = _personenListViewModel.LadeAsync();
    }

    [RelayCommand]
    private void ShowPersonenListe() => CurrentViewModel = _personenListViewModel;
}
