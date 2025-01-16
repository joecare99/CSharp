using System;
using System.Globalization;
using System.Windows.Data;

namespace MVVM_16_UserControl1.ValueConverter;

public class CurrencyValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is decimal dval)
            return dval.ToString("0.00€");
        else
            return value?.ToString()??"";

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
