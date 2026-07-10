using CommonDialogs.Helper;
using CommonDialogs.Models;
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
            System.Drawing.Font drawingFont => new System.Windows.Media.FontFamily(drawingFont.Name),
            FontDialogSelection selection => new System.Windows.Media.FontFamily(selection.ToDrawingFont().Name),
            _ => null
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
        => throw new NotImplementedException();
}
