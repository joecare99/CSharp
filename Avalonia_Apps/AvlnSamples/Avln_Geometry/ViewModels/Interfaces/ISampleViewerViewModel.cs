using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;

namespace Avln_Geometry.ViewModels.Interfaces;

public interface ISampleViewerViewModel : INotifyPropertyChanged
{
    IRelayCommand<object> SampleSelectedCommand { get; }
    IRelayCommand ZoomOutCompleteCommand { get; }

    void AddView(object geometryUsageExample);
}