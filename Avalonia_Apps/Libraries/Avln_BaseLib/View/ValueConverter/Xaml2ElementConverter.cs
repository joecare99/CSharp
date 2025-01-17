using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Markup;
using Avalonia.Markup.Xaml;

namespace Avalonia.Views.ValueConverter;

public class Xaml2ElementConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string s)
            try
            {
                return (object?)AvaloniaRuntimeXamlLoader.Parse(s);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        else
        {
            return null;
        }
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
