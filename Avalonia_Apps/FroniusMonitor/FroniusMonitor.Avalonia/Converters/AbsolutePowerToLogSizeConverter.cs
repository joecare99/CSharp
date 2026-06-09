using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace FroniusMonitor.Avalonia.Converters;

/// <summary>
/// Converts an absolute power value into a logarithmically scaled packet size for the power-flow diagram.
/// </summary>
public sealed class AbsolutePowerToLogSizeConverter : IValueConverter
{
    public double MinimumSize { get; set; } = 8d;

    public double MaximumSize { get; set; } = 24d;

    public double ReferencePowerWatts { get; set; } = 10000d;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not double fPowerWatts)
        {
            return MinimumSize;
        }

        double fAbsolutePower = Math.Abs(fPowerWatts);
        if (fAbsolutePower <= 0d)
        {
            return MinimumSize;
        }

        double fNormalizedValue = Math.Log10(1d + Math.Min(fAbsolutePower, ReferencePowerWatts)) / Math.Log10(1d + ReferencePowerWatts);
        return MinimumSize + ((MaximumSize - MinimumSize) * fNormalizedValue);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
