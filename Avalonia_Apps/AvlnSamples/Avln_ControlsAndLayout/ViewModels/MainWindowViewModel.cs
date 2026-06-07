using Avalonia.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.IO;
using System.Reflection;

namespace Avln_ControlsAndLayout.ViewModels;

/// <summary>
/// Main window view model exposing the sample view model and embedded source code.
/// </summary>
public class MainWindowViewModel : BaseViewModelCT
{
    public MainWindowViewModel() : this(Ioc.Default.GetService<ControlsAndLayoutViewModel>() ?? new ControlsAndLayoutViewModel())
    {
    }

    public MainWindowViewModel(ControlsAndLayoutViewModel controlsAndLayout)
    {
        ControlsAndLayout = controlsAndLayout;
    }

    public ControlsAndLayoutViewModel ControlsAndLayout { get; }

    public string ControlsAndLayoutViewCode => LoadEmbeddedResource("Avln_ControlsAndLayout.Views.ControlsAndLayoutView.axaml");

    public string ControlsAndLayoutViewModelCode => LoadEmbeddedResource("Avln_ControlsAndLayout.ViewModels.ControlsAndLayoutViewModel.cs");

    private static string LoadEmbeddedResource(string resourceName)
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream is null)
            {
                return $"Resource not found: {resourceName}";
            }

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
        catch (Exception ex)
        {
            return $"Error loading resource: {ex.Message}";
        }
    }
}
