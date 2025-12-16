// ***********************************************************************
// Assembly         : AA25_RichTextEdit
// ***********************************************************************
using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace AA25_RichTextEdit.ValueConverters;

public class DateTimeValueConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dt)
            return parameter is string fmt ? dt.ToString(fmt) : dt.ToString(culture);
        return value?.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string s && DateTime.TryParse(s.Trim(), culture, DateTimeStyles.AssumeLocal, out var dt))
            return dt;
        return DateTime.MinValue;
    }
}
