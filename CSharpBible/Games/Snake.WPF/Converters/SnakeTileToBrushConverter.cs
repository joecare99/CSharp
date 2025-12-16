using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Snake_Base.Models.Data;
using Snake_Base.ViewModels;

namespace Snake.WPF.Converters
{
    public class SnakeTileToBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is Point p && values[1] is ISnakeViewModel.ITileProxy<SnakeTiles> tiles)
            {
                var tile = tiles[p];
                return tile switch
                {
                    SnakeTiles.Empty => Brushes.Black,
                    SnakeTiles.Wall => Brushes.DimGray,
                    SnakeTiles.Apple => Brushes.OrangeRed,
                    SnakeTiles.SnakeHead_N or SnakeTiles.SnakeHead_S or SnakeTiles.SnakeHead_E or SnakeTiles.SnakeHead_W => Brushes.LimeGreen,
                    SnakeTiles.SnakeTail_N or SnakeTiles.SnakeTail_S or SnakeTiles.SnakeTail_E or SnakeTiles.SnakeTail_W => Brushes.Green,
                    SnakeTiles.SnakeBody_NS or SnakeTiles.SnakeBody_NE or SnakeTiles.SnakeBody_NW or SnakeTiles.SnakeBody_SE or SnakeTiles.SnakeBody_SW or SnakeTiles.SnakeBody_WE => Brushes.ForestGreen,
                    _ => Brushes.Black
                };
            }
            return Brushes.Black;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
