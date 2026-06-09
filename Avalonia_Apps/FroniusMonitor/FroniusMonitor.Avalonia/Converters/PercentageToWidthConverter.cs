using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace FroniusMonitor.Avalonia.Converters;

/// <summary>
/// Converts a percentage value into a width for the battery charge indicator.
/// </summary>
public sealed class PercentageToWidthConverter : IValueConverter
{
    public double MaximumWidth { get; set; } = 56d;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not double fPercent)
        {
            return 0d;
        }

        double fClampedPercent = Math.Max(0d, Math.Min(100d, fPercent));
        return MaximumWidth * (fClampedPercent / 100d);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
