using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace AA37_TreeView.ValueConverters;

/// <summary>
/// Converts <see cref="DateTime"/> values to and from formatted text.
/// </summary>
public sealed class DateTimeValueConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dateTime)
        {
            return parameter is string format && !string.IsNullOrWhiteSpace(format)
                ? dateTime.ToString(format, culture)
                : dateTime.ToString(culture);
        }

        return value?.ToString() ?? string.Empty;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string text && DateTime.TryParse(text.Trim(), culture, DateTimeStyles.AssumeLocal, out var result))
        {
            return result;
        }

        return DateTime.MinValue;
    }
}
