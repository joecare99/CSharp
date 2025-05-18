using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gen_FreeWin;
using Gen_FreeWin.Views;
using GenFree.ViewModels.Interfaces;
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

    public Action<object, object> onCancel { get ; set ; }
    public Action onFromFile { get ; set ; }
    public Action onReenter { get ; set ; }

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
    private void Cancel() => onCancel?.Invoke(null, null);

    [RelayCommand]
    private void Reenter() => onReenter?.Invoke();

    [RelayCommand]
    private void LoadFromFile() => onFromFile?.Invoke();

}