using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace FroniusMonitor.Avalonia.Converters;

/// <summary>
/// Converts a numeric size into a centered negative offset so animated packets stay on their path midpoint.
/// </summary>
public sealed class HalfValueToNegativeOffsetConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is double fValue
            ? -0.5d * fValue
            : 0d;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
