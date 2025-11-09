using System;
using System.Globalization;
using System.Windows.Data;

namespace DataAnalysis.WPF.Views.Converters;

public sealed class RelativeScaleConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is null || values.Length < 2)
        {
            return 0d;
        }
        try
        {
            var value = System.Convert.ToDouble(values[0], CultureInfo.InvariantCulture);
            var available = System.Convert.ToDouble(values[1], CultureInfo.InvariantCulture);
            var labelCols = 200.0 + 60.0;
            var barMax = Math.Max(0, available - labelCols);
            var scale = barMax / 100.0;
            return Math.Max(0, value * scale);
        }
        catch
        {
            return 0d;
        }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
