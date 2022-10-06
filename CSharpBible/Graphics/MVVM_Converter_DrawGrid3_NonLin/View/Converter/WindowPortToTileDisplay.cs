// ***********************************************************************
// Assembly         : MVVM_Converter_DrawGrid3_NonLin
// Author           : Mir
// Created          : 09-02-2022
//
// Last Modified By : Mir
// Last Modified On : 09-04-2022
// ***********************************************************************
// <copyright file="WindowPortToTileDisplay.cs" company="JC-Soft">
//     (c) by Joe Care 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Data;
using MVVM_Converter_DrawGrid3_NonLin.ViewModel;
using System.Collections.ObjectModel;
using System.Windows;

namespace MVVM_Converter_DrawGrid3_NonLin.View.Converter
{
    /// <summary>
    /// Class WindowPortToTileDisplay.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class WindowPortToTileDisplay : IValueConverter
    {
        /// <summary>
        /// Gets or sets the size of the window.
        /// </summary>
        /// <value>The size of the window.</value>
        public System.Windows.Size WindowSize { get; set; } = new System.Windows.Size(600,600);
        /// <summary>
        /// Gets or sets the brushes.
        /// </summary>
        /// <value>The brushes.</value>
        public List<Brush> brushes { get; set; } = new List<Brush> {};
        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        /// <value>The background.</value>
        public Brush background { get; set; } = Brushes.DarkBlue;
        /// <summary>
        /// Gets or sets the size of the tile.
        /// </summary>
        /// <value>The size of the tile.</value>
        public System.Windows.Size TileSize { get; set; } = new System.Windows.Size(32, 32);

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns <see langword="null" />, the valid null value is used.</returns>
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
                            Width = TileSize.Width,
                            Height = TileSize.Height,
                            RenderTransform = new TranslateTransform(c.tiles[i].position.X * dSize.Width, c.tiles[i].position.Y * dSize.Height),
                            Fill = brushes[c.tiles.Length == 0? i % brushes.Count: c.tiles[i].tileType]
                        };
                        result.Add(l);
                    }
                    return result;
                default: return new ObservableCollection<FrameworkElement>();
            }
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns <see langword="null" />, the valid null value is used.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
