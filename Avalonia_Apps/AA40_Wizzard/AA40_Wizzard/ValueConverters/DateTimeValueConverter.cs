using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace AA40_Wizzard.ValueConverters;

/// <summary>
/// Formats <see cref="DateTime"/> values for display.
/// </summary>
public sealed class DateTimeValueConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return string.Empty;
        }

        if (value is not DateTime dateTime)
        {
            return value;
        }

        if (parameter is string format && !string.IsNullOrWhiteSpace(format))
        {
            return dateTime.ToString(format, culture);
        }

        return dateTime.ToString(culture);
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string text)
        {
            if (parameter is string format && !string.IsNullOrWhiteSpace(format) && DateTime.TryParseExact(text, format, culture, DateTimeStyles.None, out var exactDateTime))
            {
                return exactDateTime;
            }

            if (DateTime.TryParse(text, culture, DateTimeStyles.None, out var dateTime))
            {
                return dateTime;
            }
        }

        return BindingOperations.DoNothing;
    }
}
