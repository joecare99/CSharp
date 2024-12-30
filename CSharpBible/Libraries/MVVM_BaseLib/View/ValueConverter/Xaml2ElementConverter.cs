using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace MVVM.View.ValueConverter;

public class Xaml2ElementConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string s)
            try
            {
                return (object?)XamlReader.Parse(s);
            }
            catch (Exception)
            {
                return null;
            }
        else
        {
            return null;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is FrameworkElement fw)
            return XamlWriter.Save(fw);
        else
            return "";
    }
}
