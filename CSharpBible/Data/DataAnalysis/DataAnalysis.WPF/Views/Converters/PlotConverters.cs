using System;
using System.Globalization;
using System.Windows.Data;

namespace DataAnalysis.WPF.Views.Converters;

public sealed class PlotXConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is null || values.Length < 4) return 0d;
        try
        {
            var x = System.Convert.ToDouble(values[0], CultureInfo.InvariantCulture);
            var minX = System.Convert.ToDouble(values[1], CultureInfo.InvariantCulture);
            var maxX = System.Convert.ToDouble(values[2], CultureInfo.InvariantCulture);
            var width = System.Convert.ToDouble(values[3], CultureInfo.InvariantCulture);
            if (maxX <= minX) return 0d;
            var t = (x - minX) / (maxX - minX);
            return Math.Max(0, Math.Min(width, t * width));
        }
        catch
        {
            return 0d;
        }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotSupportedException();
}

public sealed class PlotYConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is null || values.Length < 4) return 0d;
        try
        {
            var y = System.Convert.ToDouble(values[0], CultureInfo.InvariantCulture);
            var minY = System.Convert.ToDouble(values[1], CultureInfo.InvariantCulture);
            var maxY = System.Convert.ToDouble(values[2], CultureInfo.InvariantCulture);
            var height = System.Convert.ToDouble(values[3], CultureInfo.InvariantCulture);
            if (maxY <= minY) return 0d;
            var t = (y - minY) / (maxY - minY);
            return Math.Max(0, Math.Min(height, (1 - t) * height));
        }
        catch
        {
            return 0d;
        }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotSupportedException();
}

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
        catch
        {
            return MinSize;
        }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotSupportedException();
}
