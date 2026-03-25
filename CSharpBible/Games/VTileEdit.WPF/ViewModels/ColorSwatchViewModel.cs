using System;
using System.Windows.Media;
using VTileEdit.WPF.Helpers;

namespace VTileEdit.WPF.ViewModels;

/// <summary>
/// Represents a selectable console color within the palette.
/// </summary>
public partial class ColorSwatchViewModel : ObservableObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColorSwatchViewModel"/> class.
    /// </summary>
    /// <param name="color">The console color represented by the swatch.</param>
    public ColorSwatchViewModel(ConsoleColor color)
    {
        Color = color;
        Brush = ConsoleColorBrushCache.GetBrush(color);
        Name = color.ToString();
    }

    /// <summary>
    /// Gets the represented console color.
    /// </summary>
    public ConsoleColor Color { get; }

    /// <summary>
    /// Gets the visual brush for UI bindings.
    /// </summary>
    public Brush Brush { get; }

    /// <summary>
    /// Gets the display name shown in the palette.
    /// </summary>
    public string Name { get; }

    [ObservableProperty]
    private bool isForegroundSelection;

    [ObservableProperty]
    private bool isBackgroundSelection;
}
