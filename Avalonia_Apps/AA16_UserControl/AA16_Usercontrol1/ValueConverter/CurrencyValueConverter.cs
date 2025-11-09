using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace AA16_UserControl1.ValueConverter;

public class CurrencyValueConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is decimal dval)
            return dval.ToString("0.00€");
        else
            return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
