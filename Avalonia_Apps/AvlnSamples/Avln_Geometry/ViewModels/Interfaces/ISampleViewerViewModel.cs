using CommunityToolkit.Mvvm.Input;

namespace Avln_Geometry.ViewModels.Interfaces;

public interface ISampleViewerViewModel
{
    IRelayCommand<object>  SampleSelectedCommand { get; }
    IRelayCommand ZoomOutCompleteCommand { get; }

    void AddView(object geometryUsageExample);
}