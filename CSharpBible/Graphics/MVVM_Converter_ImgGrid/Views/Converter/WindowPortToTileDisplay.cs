using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Data;
using MVVM_Converter_ImgGrid.ViewModel;
using System.Collections.ObjectModel;
using System.Windows;

namespace MVVM_Converter_ImgGrid.Views.Converter
{
    public class WindowPortToTileDisplay : IValueConverter
    {
        public System.Windows.Size WindowSize { get; set; } = new System.Windows.Size(600,600);
        public List<Brush> brushes { get; set; } = new List<Brush> {};
        public Brush background { get; set; } = Brushes.DarkBlue;
        /// <summary>
        /// Gets or sets the size of the tile.
        /// </summary>
        /// <value>The size of the tile.</value>
        public System.Windows.Size TileSize { get; set; } = new System.Windows.Size(32, 32);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //int w;
            switch (value)
            {
                case TileData[] c when parameter is string s:
                    System.Drawing.Size dSize = System.Drawing.Size.Empty;
                    if (int.TryParse(s, out var w))
                        dSize = new System.Drawing.Size(w, w);
                    else if (s.Contains(";"))
                    {
                        var aS = s.Split(';');
                        if (int.TryParse(aS[0], out var x) && int.TryParse(aS[1], out var y))
                            dSize = new System.Drawing.Size(x, y);
                    }

                    ObservableCollection<FrameworkElement> result = new ObservableCollection<FrameworkElement>();


                    for (var i = 0; i < c.Length; i++)
                    {
                        FrameworkElement l = new System.Windows.Shapes.Rectangle()
                        {
                            Width = TileSize.Width,
                            Height = TileSize.Height,
                            RenderTransform = new TranslateTransform(c[i].position.X * dSize.Width, c[i].position.Y * dSize.Height),
                            Fill = brushes[c[i].tileType < brushes.Count ? c[i].tileType : 0]
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
