using CommunityToolkit.Mvvm.Input;
using Gen_FreeWin;
using System;
using System.ComponentModel;

namespace GenFree.ViewModels.Interfaces;

public interface IFraPersImpQueryViewModel : INotifyPropertyChanged
{
    IRelayCommand DeleteQuietCommand { get; }
    IRelayCommand CancelCommand { get; }
    IRelayCommand ReenterCommand { get; }
    IRelayCommand LoadFromFileCommand { get; }

    EUserText IText { get; set; }
    EUserText IReenter { get; set; }
    EUserText ICancel { get; set; }
    EUserText IDelete { get; set; }
    EUserText ILoadFromFile { get; set; }
    Action<object, object> onCancel { get; set; }
    Action onFromFile { get; set; }
    Action onReenter { get; set; }

    void SetDefaultTexts();
}