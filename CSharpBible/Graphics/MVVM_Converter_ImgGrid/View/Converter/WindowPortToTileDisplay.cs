using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using MVVM_Converter_ImgGrid.ViewModel;
using System.Collections.ObjectModel;
using System.Windows;

namespace MVVM_Converter_ImgGrid.View.Converter
{
    public class WindowPortToTileDisplay : IValueConverter
    {
        public System.Windows.Size WindowSize { get; set; } = new System.Windows.Size(600,600);
        public List<Brush> brushes { get; set; } = new List<Brush> {};
        public Brush background { get; set; } = Brushes.DarkBlue;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //int w;
            switch (value)
            {
                case SWindowPort c when parameter is string s:
                    System.Drawing.Size dSize = System.Drawing.Size.Empty;
                    if (int.TryParse(s, out var w))
                        dSize = new System.Drawing.Size(w, w);
                    else if (s.Contains(";"))
                    {
                        var aS = s.Split(";");
                        if (int.TryParse(aS[0], out var x) && int.TryParse(aS[1], out var y))
                            dSize = new System.Drawing.Size(x, y);
                    }

                    ObservableCollection<FrameworkElement> result = new ObservableCollection<FrameworkElement>();


                    for (var i = 0; i < c.tiles.Length; i++)
                    {
                        FrameworkElement l = new System.Windows.Shapes.Rectangle()
                        {
                            Width = w,
                            Height = w,
                            RenderTransform = new TranslateTransform(c.tiles[i].position.X * dSize.Width, c.tiles[i].position.Y * dSize.Height),
                            Fill = brushes[c.tiles[i].tileType]
                        };
                        result.Add(l);
                    }
                    return result;
                default: return new ObservableCollection<FrameworkElement>();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
