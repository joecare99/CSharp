using System;
using System.Globalization;
using System.Windows.Data;

namespace MVVM_41_Sudoku.Views.Converter;

public class DoubleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value switch
    {
        double d when double.TryParse(parameter?.ToString(),NumberStyles.Any,CultureInfo.InvariantCulture, out double p) => d * p,
        double d => d,
        _ => throw new ArgumentException("Value must be a double")
    };

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
