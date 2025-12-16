using System;
using System.Globalization;
using System.Windows.Data;

namespace DataAnalysis.WPF.Views.Converters;

public sealed class PlotConverter : IMultiValueConverter
{
    // Half of maximum bubble size (SizeByCountConverter.MaxSize) to keep bubbles inside plot.
    private const double EdgePadding = 60d; // MaxSize (120) / 2
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is null || values.Length < 4) return 0d;
        try
        {
            var x = System.Convert.ToDouble(values[0], CultureInfo.InvariantCulture);
            var minX = System.Convert.ToDouble(values[1], CultureInfo.InvariantCulture);
            var maxX = System.Convert.ToDouble(values[2], CultureInfo.InvariantCulture);
            var width = System.Convert.ToDouble(values[3], CultureInfo.InvariantCulture);
            var Offset = parameter != null?System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture):0d;
            if (width <= 0) return 0d;
            if (maxX == minX)
            {
                // Degenerate range: place at center with padding consideration
                var usable = Math.Max(0, width - 2 * EdgePadding);
                return EdgePadding + usable / 2;
            }
            var usableWidth = Math.Max(0, width - 2 * EdgePadding);
            var t = (x - minX) / (maxX - minX);
            t = Math.Max(0, Math.Min(1, t));
            return EdgePadding + t * usableWidth+Offset;
        }
        catch
        {
            return 0d;
        }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotSupportedException();
}
