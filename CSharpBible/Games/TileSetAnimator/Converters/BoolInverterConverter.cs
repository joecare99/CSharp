using System;
using System.Globalization;
using System.Windows.Data;

namespace TileSetAnimator.Converters;

/// <summary>
/// Inverts a boolean value.
/// </summary>
public sealed class BoolInverterConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        => value is bool b ? !b : true;

    public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
