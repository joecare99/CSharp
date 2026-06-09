using System;
using Avalonia.Data.Converters;
using System.Globalization;

namespace AA28_DataGridExt.ValueConverter;

/// <summary>
/// Converts boolean values to and from Avalonia visibility booleans.
/// </summary>
public class Bool2VisibilityConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool isVisible ? isVisible : true;

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool isVisible && isVisible;
}
