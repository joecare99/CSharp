using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Avln_Bubbles.View.Converters;

/// <summary>
/// Converts a boolean selection flag into a border thickness.
/// </summary>
public sealed class BoolToDoubleThicknessConverter : IValueConverter
{
    /// <summary>
    /// Gets the shared instance.
    /// </summary>
    public static BoolToDoubleThicknessConverter Instance { get; } = new();

    /// <inheritdoc/>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? new Thickness(3) : new Thickness(0);

    /// <inheritdoc/>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
