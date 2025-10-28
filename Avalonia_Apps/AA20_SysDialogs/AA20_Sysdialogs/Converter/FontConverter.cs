using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace AA20_SysDialogs.Converter;

public class FontConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
#pragma warning disable CA1416 // Platform compatibility
        return value switch
        {
            System.Drawing.Font f => new FontFamily(f.Name),
            _ => null
        };
#pragma warning restore CA1416
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
