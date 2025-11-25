using System.Globalization;
using System.Windows.Data;

namespace DataAnalysis.WPF.Views.Converters;

public sealed class PlotYCenteredConverter : IMultiValueConverter
{
    private const double EdgePadding = 60d;
    private const double MinSize = 6d;
    private const double MaxSize = 120d;
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is null || values.Length < 6) return 0d;
        try
        {
            var y = System.Convert.ToDouble(values[0], CultureInfo.InvariantCulture);
            var minY = System.Convert.ToDouble(values[1], CultureInfo.InvariantCulture);
            var maxY = System.Convert.ToDouble(values[2], CultureInfo.InvariantCulture);
            var height = System.Convert.ToDouble(values[3], CultureInfo.InvariantCulture);
            var count = System.Convert.ToDouble(values[4], CultureInfo.InvariantCulture);
            var maxCount = System.Convert.ToDouble(values[5], CultureInfo.InvariantCulture);
            if (height <= 0) return 0d;
            double size = MinSize;
            if (maxCount > 0)
            {
                var tSize = Math.Max(0, Math.Min(1, count / maxCount));
                tSize = Math.Sqrt(tSize);
                size = MinSize + (MaxSize - MinSize) * tSize;
            }
            var radius = size / 2d;
            if (maxY <= minY)
            {
                return Math.Max(0, Math.Min(height - size, height / 2d - radius));
            }
            var usableHeight = Math.Max(0, height - 2 * EdgePadding);
            var t = (y - minY) / (maxY - minY);
            t = Math.Max(0, Math.Min(1, t));
            var center = EdgePadding + (1 - t) * usableHeight; // invert y
            var top = center - radius;
            if (top < 0) top = 0;
            if (top + size > height) top = Math.Max(0, height - size);
            return top;
        }
        catch { return 0d; }
    }
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotSupportedException();
}
