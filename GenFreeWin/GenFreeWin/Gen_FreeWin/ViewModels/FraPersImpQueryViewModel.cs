using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GenFree.ViewModels.Interfaces;
using System;

namespace GenFreeWin.ViewModels;

public partial class FraPersImpQueryViewModel : ObservableObject, IFraPersImpQueryViewModel
{

    [ObservableProperty]
    public partial EUserText IText { get; set; }

    [ObservableProperty]
    public partial EUserText IReenter { get; set; }
    [ObservableProperty]
    public partial EUserText ICancel { get; set; }
    [ObservableProperty]
    public partial EUserText IDelete { get; set; }
    [ObservableProperty]
    public partial EUserText ILoadFromFile { get; set; }
    public Action<object, object> onCancel { get; set; }
    public Action onFromFile { get; set; }
    public Action onDelete { get; set; }
    public Action onReenter { get; set; }

    public void SetDefaultTexts()
    {
    }

    [RelayCommand]
    private void DeleteQuiet() => onDelete?.Invoke();

    [RelayCommand]
    private void Cancel() => onCancel(this, null);

    [RelayCommand]
    private void Reenter() => onReenter?.Invoke();


    [RelayCommand]
    private void LoadFromFile() => onFromFile?.Invoke();


}
