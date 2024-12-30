// ***********************************************************************
// Assembly         : MVVM_Converter_Grid2
// Author           : Mir
// Created          : 08-20-2022
//
// Last Modified By : Mir
// Last Modified On : 08-20-2022
// ***********************************************************************
// <copyright file="DrawScreenObjects.cs" company="JC-Soft">
//     (c) by Joe Care 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows;
using Asteroids.Model.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Asteroids.Views.Converter;

/// <summary>
/// Class DrawScreenObjects.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class DrawScreenObjects : IValueConverter
{

    /// <summary>
    /// Gets or sets the size of the window [Pixel].
    /// </summary>
    /// <value>[Pixel] The size of the window.</value>
    public System.Windows.Size WindowSize { get; set; } = new System.Windows.Size(1600, 900);
    public Rect Port { get; set; } = new System.Windows.Rect(0,0,1600, 900);

    /// <summary>
    /// Converts koordinates from Real to Visual[Pixel].
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="visMin">The vis minimum.</param>
    /// <param name="visMax">The vis maximum.</param>
    /// <param name="rMin">The r minimum.</param>
    /// <param name="rMax">The r maximum.</param>
    /// <returns>System.Double</returns>
    private double Real2Vis(double value, double visMin, double visMax, double rMin, double rMax)
        => visMin + (value - rMin) * (visMax - visMin) / (rMax - rMin);

    /// <summary>
    /// Converts koordinates from Real to Visual[Pixel].
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="port">The port.</param>
    /// <returns>System.Windows.Point</returns>
    private System.Windows.Point real2VisP(Point value, Rect port) =>
       new System.Windows.Point(Real2Vis(value.X, 0, WindowSize.Width, port.Left, port.Right),
           Real2Vis(value.Y, WindowSize.Height, 0d, port.Top, port.Bottom) );

    /// <summary>
    /// standard converter-function
    /// </summary>
    public Func<Point, Rect, System.Windows.Point> Real2VisP;

    /// <summary>
    /// Initializes a new instance of the <see cref="DrawScreenObjects"/> class.
    /// </summary>
    public DrawScreenObjects()
    {
        Real2VisP = real2VisP;
    }

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
        switch (value)
        {
            case IEnumerable<IScreenObj> ds:
                ObservableCollection<FrameworkElement> _fwe = new ObservableCollection<FrameworkElement>();
                foreach(var _itm in ds.ToList())
                {
                    if (_itm.xOutline)
                    {
                        _fwe.Add(CreatePolygon(_itm, Colors.Black, 3));
                    }

                    if (_itm.Points.Length>1)
                    _fwe.Add(CreatePolygon(_itm, _itm.color, 1));
                    else if (_itm.Points.Length == 1)
                        _fwe.Add(CreateDot(_itm, _itm.color, 1));

                }
                return _fwe;
            default: return new ObservableCollection<FrameworkElement>();
        }

        FrameworkElement CreatePolygon(IScreenObj _itm, Color _col, double _wdt)
        {
            return new Polyline()
            {
                Points = new PointCollection(_itm.Points.Select(p => Real2VisP(p, this.Port)).ToArray()),
                Stroke = new SolidColorBrush(_col),
                StrokeLineJoin = PenLineJoin.Round,
                StrokeThickness = _wdt,
            };
        }

        FrameworkElement CreateDot(IScreenObj _itm, Color _col, double _wdt)
        {
            return new Polyline()
            {
                Points = new PointCollection()
                {
                    Real2VisP(_itm.Points[0], this.Port),
                    Real2VisP(_itm.Points[0], this.Port) + new Vector(1,0),
                },
                Stroke = new SolidColorBrush(_col),
                StrokeLineJoin = PenLineJoin.Round,
                StrokeThickness = _wdt,
            };
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
    /// <exception cref="NotImplementedException"></exception>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
