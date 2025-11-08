using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DataAnalysis.WPF.Views.Converters;

public sealed class HeatmapBrushConverter : IMultiValueConverter
{
 public Brush LowBrush { get; set; } = new SolidColorBrush(Color.FromRgb(229,243,255)); // light
 public Brush HighBrush { get; set; } = new SolidColorBrush(Color.FromRgb(16,92,172)); // dark

 public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
 {
 try
 {
 if (values is null || values.Length <2)
 return Brushes.Transparent;

 // Cell value
 var v = System.Convert.ToDouble(values[0], CultureInfo.InvariantCulture);
 // Max value from VM
 var max = System.Convert.ToDouble(values[1], CultureInfo.InvariantCulture);
 if (max <=0)
 return Brushes.Transparent;

 var t = Math.Max(0.0, Math.Min(1.0, v / max));
 // simple linear interpolation between LowBrush and HighBrush
 Color c1 = ((SolidColorBrush)LowBrush).Color;
 Color c2 = ((SolidColorBrush)HighBrush).Color;
 byte Lerp(byte a, byte b) => (byte)(a + (b - a) * t);
 var c = Color.FromRgb(Lerp(c1.R, c2.R), Lerp(c1.G, c2.G), Lerp(c1.B, c2.B));
 return new SolidColorBrush(c);
 }
 catch
 {
 return Brushes.Transparent;
 }
 }

 public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
 {
 throw new NotSupportedException();
 }
}
