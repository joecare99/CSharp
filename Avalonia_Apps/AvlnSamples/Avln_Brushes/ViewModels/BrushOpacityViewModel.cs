using CommunityToolkit.Mvvm.ComponentModel;

namespace Avln_Brushes.ViewModels;

public partial class BrushOpacityViewModel : ObservableObject
{
    // Opacity values for the examples
    public double[] OpacityValues { get; } = [1.0, 0.75, 0.5, 0.25, 0.0];
    
    public string[] OpacityLabels { get; } = ["1.0", "0.75", "0.5", "0.25", "0.0"];
    
    public string[] BrushTypeLabels { get; } = 
    [
  "SolidColorBrush",
        "LinearGradientBrush",
        "RadialGradientBrush",
        "ImageBrush",
  "DrawingBrush"
    ];
}
