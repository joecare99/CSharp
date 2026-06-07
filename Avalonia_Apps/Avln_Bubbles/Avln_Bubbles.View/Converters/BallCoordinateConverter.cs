using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Avln_Bubbles.View.Converters;

/// <summary>
/// Converts board coordinates into pixel offsets for the canvas layout.
/// </summary>
public sealed class BallCoordinateConverter : IValueConverter
{
    /// <summary>
    /// Gets the shared converter instance.
    /// </summary>
    public static BallCoordinateConverter Instance { get; } = new();

    /// <summary>
    /// Gets or sets the cell size used for rendering.
    /// </summary>
    public double CellSize { get; set; } = 52d;

    /// <inheritdoc/>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int coordinate)
        {
            return coordinate * CellSize;
        }

        return 0d;
    }

    /// <inheritdoc/>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
