using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;

namespace Geometry.ViewModels.Interfaces;

public interface ISampleViewerViewModel : INotifyPropertyChanged
{
    object ActView { get; }

    void AddView(object geometryUsageExample);

    IRelayCommand<object> SampleSelectedCommand { get; }
    IRelayCommand ZoomOutCompleteCommand { get; }

    event EventHandler ZoomOut;
}