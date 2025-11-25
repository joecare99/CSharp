using System.Globalization;
using System.Windows.Data;

namespace DataAnalysis.WPF.Views.Converters;

// Centered converters: return Canvas.Left/Top such that given (X,Y) is bubble center.
// Expect bindings: X|minX|maxX|width|count|maxCount (6 values) for X, similarly Y.
public sealed class PlotXCenteredConverter : IMultiValueConverter
{
    private const double EdgePadding = 60d; // Align with PlotConverter padding
    private const double MinSize = 6d;
    private const double MaxSize = 120d;
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is null || values.Length < 6) return 0d;
        try
        {
            var x = System.Convert.ToDouble(values[0], CultureInfo.InvariantCulture);
            var minX = System.Convert.ToDouble(values[1], CultureInfo.InvariantCulture);
            var maxX = System.Convert.ToDouble(values[2], CultureInfo.InvariantCulture);
            var width = System.Convert.ToDouble(values[3], CultureInfo.InvariantCulture);
            var count = System.Convert.ToDouble(values[4], CultureInfo.InvariantCulture);
            var maxCount = System.Convert.ToDouble(values[5], CultureInfo.InvariantCulture);
            if (width <= 0) return 0d;
            double size = MinSize;
            if (maxCount > 0)
            {
                var tSize = Math.Max(0, Math.Min(1, count / maxCount));
                tSize = Math.Sqrt(tSize);
                size = MinSize + (MaxSize - MinSize) * tSize;
            }
            var radius = size / 2d;
            if (maxX == minX)
            {
                return Math.Max(0, Math.Min(width - size, width / 2d - radius));
            }
            var usableWidth = Math.Max(0, width - 2 * EdgePadding);
            var t = (x - minX) / (maxX - minX);
            t = Math.Max(0, Math.Min(1, t));
            var center = EdgePadding + t * usableWidth;
            var left = center - radius;
            // Clamp to keep bubble fully visible
            if (left < 0) left = 0;
            if (left + size > width) left = Math.Max(0, width - size);
            return left;
        }
        catch { return 0d; }
    }
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotSupportedException();
}
