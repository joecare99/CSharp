using System;
using System.Globalization;
using System.Windows.Data;

namespace MVVM_20_Sysdialogs.Converter;

public class FontConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            System.Drawing.Font f => new System.Windows.Media.FontFamily(f.Name),
            _ => null
        } ;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
        => throw new NotImplementedException();
}
