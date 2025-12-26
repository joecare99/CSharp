using System;
using System.Globalization;
using System.Windows.Data;

namespace TileSetAnimator.Converters;

/// <summary>
/// Converts null references to boolean values (null => false).
/// </summary>
public sealed class NullToBooleanConverter : IValueConverter
{
    /// <summary>
    /// Gets or sets a value indicating whether the result should be inverted.
    /// </summary>
    public bool Invert { get; set; }

    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        var result = value != null;
        return Invert ? !result : result;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
