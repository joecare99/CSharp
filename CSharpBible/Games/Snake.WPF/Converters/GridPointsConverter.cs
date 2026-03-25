using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;

namespace Snake.WPF.Converters
{
    public class GridPointsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Size s)
            {
                var points = new List<Point>(s.Width * s.Height);
                for (int y = 0; y < s.Height; y++)
                    for (int x = 0; x < s.Width; x++)
                        points.Add(new Point(x-1, y-1));
                return points;
            }
            return Array.Empty<Point>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
