using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avln_Brushes.ViewModels.Interfaces;
using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;

namespace Avln_Brushes.ViewModels;

public partial class SampleViewerViewModel : ObservableObject, ISampleViewerViewModel
{
    [ObservableProperty]
    private object? _currentView;

    public SampleViewerViewModel()
    {
        // Default to GradientBrushesExample
        CurrentView = new GradientBrushesViewModel();
    }

    public event EventHandler? DoExit;

    public ICommand? TransitionCommand => TransitionRelayCommand;

    [RelayCommand]
    private void Exit()
    {
        DoExit?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void TransitionRelay()
    {
        // Implement view transition logic here
        // For now, cycle between different examples
    }

    [RelayCommand]
    private void ShowGradientBrushes()
    {
        CurrentView = new GradientBrushesViewModel();
    }

    [RelayCommand]
    private void ShowInteractiveGradient()
    {
        CurrentView = new InteractiveLinearGradientViewModel();
    }

    [RelayCommand]
    private void ShowDashExample()
    {
        CurrentView = new DashExampleViewModel();
    }

    [RelayCommand]
    private void ShowPredefinedBrushes()
    {
        CurrentView = new PredefinedBrushesViewModel();
    }

    [RelayCommand]
    private void ShowBrushTransform()
    {
        CurrentView = new BrushTransformViewModel();
    }
}

