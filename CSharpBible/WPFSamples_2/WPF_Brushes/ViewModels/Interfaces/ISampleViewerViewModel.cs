using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;

namespace Brushes.ViewModels.Interfaces;

public interface ISampleViewerViewModel : INotifyPropertyChanged
{
    string NavigationSource { get; }

    IRelayCommand<object> SampleSelectedCommand { get; }
    IRelayCommand TransitionCommand { get; }
    IRelayCommand<object> SelectedItemChangedCommand { get; }
    IRelayCommand ExitCommand { get; }

    event EventHandler ZoomOut;
    event EventHandler DoExit;
}