using System.Globalization;
using System.Windows.Data;

namespace DataAnalysis.WPF.Views.Converters;

public sealed class SizeByCountConverter : IMultiValueConverter
{
    public double MinSize { get; set; } = 6;
    public double MaxSize { get; set; } = 120;
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is null || values.Length < 2) return MinSize;
        try
        {
            var count = System.Convert.ToDouble(values[0], CultureInfo.InvariantCulture);
            var max = System.Convert.ToDouble(values[1], CultureInfo.InvariantCulture);
            if (max <= 0) return MinSize;
            var t = Math.Max(0, Math.Min(1, count / max));
            t = Math.Sqrt(t);
            return MinSize + (MaxSize - MinSize) * t;
        }
        catch { return MinSize; }
    }
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotSupportedException();
}
