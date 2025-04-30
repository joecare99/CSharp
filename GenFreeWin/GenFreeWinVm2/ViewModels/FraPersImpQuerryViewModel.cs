using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gen_FreeWin;
using Gen_FreeWin.Views;
using MVVM.ViewModel;

namespace GenFreeWin.ViewModels;
public partial class FraPersImpQuerryViewModel : BaseViewModelCT, IFraPersImpQuerryViewModel
{
    [ObservableProperty]
    private EUserText _iText;

    [ObservableProperty]
    private EUserText _iReenter = EUserText.t72;

    [ObservableProperty]
    private EUserText _iCancel = EUserText.tNMCancel;

    [ObservableProperty]
    private EUserText _iDelete = EUserText.t307;

    [ObservableProperty]
    private EUserText _iLoadFromFile = EUserText.t306;

    public void SetDefaultTexts()
    {
        IReenter  = EUserText.t72;
        ILoadFromFile  = EUserText.t306;
        IDelete = EUserText.t307;
        ICancel = EUserText.tNMCancel;
    }

    [RelayCommand]
    private void DeleteQuiet()
    { 
    }

    [RelayCommand]
    private void Cancel() { }

    [RelayCommand]
    private void Reenter() { }

    [RelayCommand]
    private void LoadFromFile() { }

}