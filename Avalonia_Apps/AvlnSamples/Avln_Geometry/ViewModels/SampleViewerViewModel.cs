using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avln_Geometry.ViewModels.Interfaces;
using System.Collections.Generic;

namespace Avln_Geometry.ViewModels;

public partial class SampleViewerViewModel : ObservableObject, ISampleViewerViewModel
{
    private readonly List<object> _examples = new();

    [ObservableProperty]
    private object? _actView;

    [RelayCommand]
    private void SampleSelected(object? index)
    {
        if (index != null && int.TryParse(index.ToString(), out int sampleIndex))
        {
            if (sampleIndex >= 0 && sampleIndex < _examples.Count)
            {
                ActView = _examples[sampleIndex];
            }
        }
    }

    [RelayCommand]
    private void ZoomOutComplete()
    {
        // Set first view as default if ActView is null
        if (ActView == null && _examples.Count > 0)
        {
            ActView = _examples[0];
        }
    }

    public void AddView(object view)
    {
        _examples.Add(view);
    }
}

