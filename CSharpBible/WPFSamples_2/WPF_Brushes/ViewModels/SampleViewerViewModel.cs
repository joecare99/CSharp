using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Brushes.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows;

namespace Brushes.ViewModels;

public partial class SampleViewerViewModel : ObservableObject, ISampleViewerViewModel
{
    private int _sampleIndex;
    public SampleViewerViewModel()
    {
    }

    [ObservableProperty]
    private string _navigationSource = "BrushTypesExample.xaml";

    private string _preNavigationSource;

    public event EventHandler ZoomOut;

    public event EventHandler DoExit;

    [RelayCommand]
    private void SampleSelected(object index)
    {
        _sampleIndex = int.Parse($"0{index}"); 
        ZoomOut?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void SelectedItemChanged(object index)
    {
        var Sample = index as RoutedPropertyChangedEventArgs<object>;
        var _value = Sample?.NewValue as XmlElement; 
        var sampleuri = _value?.Attributes["Uri"];
        _preNavigationSource =  sampleuri?.Value;
    }

    [RelayCommand]
    private void Exit()
    {
        DoExit?.Invoke(this,EventArgs.Empty);
    }

    [RelayCommand]
    private void Transition()
    {
        NavigationSource = _preNavigationSource;
    }

}
