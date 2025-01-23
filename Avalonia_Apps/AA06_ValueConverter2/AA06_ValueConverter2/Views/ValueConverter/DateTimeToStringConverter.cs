using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace AA06_ValueConverter2.Views.ValueConverter
{
    internal class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return new Avalonia.Data.BindingNotification(value);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return new Avalonia.Data.BindingNotification(value);
        }
    }
}
