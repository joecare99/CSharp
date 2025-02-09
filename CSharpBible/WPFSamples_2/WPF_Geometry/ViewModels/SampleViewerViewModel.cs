using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Geometry.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry.ViewModels;

public partial class SampleViewerViewModel : ObservableObject, ISampleViewerViewModel
{
    private readonly List<object> _examples = new();
    private int _sampleIndex;
    public SampleViewerViewModel()
    {
    }

    [ObservableProperty]
    private object _actView;

    public event EventHandler ZoomOut;

    [RelayCommand]
    private void SampleSelected(object index)
    {
        _sampleIndex = int.Parse($"0{index}"); 
        ZoomOut?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void ZoomOutComplete()
    {
        ActView = _examples[_sampleIndex];
    }

    public void AddView(object geometryUsageExample)
    {
        _examples.Add(geometryUsageExample);
    }
}
