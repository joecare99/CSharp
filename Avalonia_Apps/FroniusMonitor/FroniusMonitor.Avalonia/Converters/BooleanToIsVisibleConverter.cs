using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace FroniusMonitor.Avalonia.Converters;

/// <summary>
/// Converts a boolean value into a boolean visibility flag for Avalonia controls.
/// </summary>
public sealed class BooleanToIsVisibleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool xVisible && xVisible;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool xVisible && xVisible;
    }
}
