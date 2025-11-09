using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Avln_Brushes.ViewModels;

public partial class GradientBrushesViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _showGradientStops = true;

    [ObservableProperty]
    private double _gradientOpacity = 1.0;

    public ObservableCollection<GradientExample> Examples { get; } = new()
 {
        new GradientExample { Name = "Diagonal Linear Gradient", Type = GradientType.DiagonalLinear },
  new GradientExample { Name = "Horizontal Linear Gradient", Type = GradientType.HorizontalLinear },
        new GradientExample { Name = "Vertical Linear Gradient", Type = GradientType.VerticalLinear },
        new GradientExample { Name = "Radial Gradient", Type = GradientType.Radial },
    new GradientExample { Name = "Condensed Horizontal", Type = GradientType.CondensedHorizontal }
    };
}

public class GradientExample
{
    public string Name { get; set; } = string.Empty;
    public GradientType Type { get; set; }
}

public enum GradientType
{
    DiagonalLinear,
    HorizontalLinear,
    VerticalLinear,
    Radial,
    CondensedHorizontal
}
