﻿// ***********************************************************************
// Assembly         : MVVM_Converter_Grid
// Author           : Mir
// Created          : 07-21-2022
//
// Last Modified By : Mir
// Last Modified On : 08-21-2022
// ***********************************************************************
// <copyright file="WindowPortToGridLines.cs" company="JC-Soft">
//     (c) by Joe Care 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using MVVM_Converter_Grid.ViewModel;
using System.Collections.ObjectModel;
using System.Windows;

namespace MVVM_Converter_Grid.View.Converter
{
    /// <summary>
    /// Class WindowPortToGridLines.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class WindowPortToGridLines : IValueConverter
    {
        /// <summary>
        /// The average GRD pixel
        /// </summary>
        const int AvgGrdPixel = 40;
        /// <summary>
        /// The lb
        /// </summary>
        System.Windows.Size lb = new System.Windows.Size(50, 28);

        /// <summary>
        /// Gets or sets the size of the window.
        /// </summary>
        /// <value>The size of the window.</value>
        public System.Windows.Size WindowSize { get; set; } = new System.Windows.Size(600,600);

        /// <summary>
        /// Real2s the vis.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="visMin">The vis minimum.</param>
        /// <param name="visMax">The vis maximum.</param>
        /// <param name="rMin">The r minimum.</param>
        /// <param name="rMax">The r maximum.</param>
        /// <returns>System.Double.</returns>
        private double Real2Vis(double value, double visMin, double visMax,double rMin,double rMax)
            => visMin + (value - rMin) * (visMax- visMin) / (rMax-rMin);

        /// <summary>
        /// Real2s the vis p.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="port">The port.</param>
        /// <returns>System.Windows.Point.</returns>
        private System.Windows.Point real2VisP(PointF value, RectangleF port) =>
           new System.Windows.Point(Real2Vis(value.X, 0, WindowSize.Width-lb.Width, port.Left, port.Right)+lb.Width,
               Real2Vis(value.Y, WindowSize.Height-lb.Height, 0d, port.Top, port.Bottom)+lb.Height);

        /// <summary>
        /// The real2 vis p
        /// </summary>
        public Func<PointF, RectangleF, System.Windows.Point> Real2VisP;

        /// <summary>
        /// Real2s the vis x.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="xMin">The x minimum.</param>
        /// <param name="xMax">The x maximum.</param>
        /// <returns>System.Double.</returns>
        private double Real2VisX(double value, double xMin, double xMax) =>
            Real2Vis(value, WindowSize.Width,0, xMin, xMax);
        /// <summary>
        /// Real2s the vis y.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="yMin">The y minimum.</param>
        /// <param name="yMax">The y maximum.</param>
        /// <returns>System.Double.</returns>
        private double Real2VisY(double value, double yMin, double yMax) =>
            Real2Vis(value, WindowSize.Height,0, yMin, yMax);

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowPortToGridLines"/> class.
        /// </summary>
        public WindowPortToGridLines()
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
                case SWindowPort c:
                    var b = new SolidColorBrush(Colors.Black);

                    var hOffset = 0d;
                    RectangleF port2; // erweiterter Viewport
                    if (Math.Abs(WindowSize.Width * c.port.Height) < Math.Abs(WindowSize.Height * c.port.Width))
                       port2 = new RectangleF(c.port.Left,(float)(c.port.Top + c.port.Height*0.5f - (c.WindowSize.Height*0.5f) * c.port.Width / (float)WindowSize.Width), c.port.Width, (float)(c.WindowSize.Height* c.port.Width/ WindowSize.Width));
                    else
                       port2 = new RectangleF((float)(c.port.Left + c.port.Width *0.5- (c.WindowSize.Width / 2) * c.port.Height / WindowSize.Height), c.port.Top , (float)(WindowSize.Width * c.port.Height / WindowSize.Height), c.port.Height);

                    double BigStep = Math.Pow(10, Math.Floor(Math.Log10(port2.Width * AvgGrdPixel * 10 / WindowSize.Width)));
                    while ((BigStep / port2.Width * WindowSize.Width) < AvgGrdPixel*3/1.5)
                        BigStep *= 2d;
                    while ((BigStep / port2.Width * WindowSize.Width) > AvgGrdPixel*4.5)
                        BigStep *= 0.5d;
                    double Step;
                    switch((BigStep / port2.Width * WindowSize.Width)/ AvgGrdPixel)
                    {
                        case double d when d > 3.5: Step = BigStep / 10;break;
                        case double d when d < 2.5:
                            Step = BigStep / 4; break;
                        default: Step = BigStep / 5;break;
                    }
                   // double Step = BigStep;//Math.Pow(10,Math.Floor(Math.Log10(BigStep/3)));

                    double MinStepX = Math.Ceiling(port2.Left / Step) * Step;
                    double MinStepY = Math.Ceiling(port2.Top / Step) * Step;

                    ObservableCollection<FrameworkElement> result = new ObservableCollection<FrameworkElement>();
                    if (c.port.Contains(PointF.Empty))
                    {
                        var p = Real2VisP(PointF.Empty, port2);
                        Ellipse el = new Ellipse() { Height = 7, Width = 7,Margin = new Thickness(p.X-3,p.Y-3,0,0), Stroke = b, StrokeThickness = 0.3d };
                        result.Add(el);
                    }

//                    var MaxSize = WindowSize.Width > (WindowSize.Height- hOffset) *1.5 ? (WindowSize.Width-lb.Width) / 1.5 : (WindowSize.Height-lb.Height- hOffset) ;
                   
                    for (var i = 0; MinStepX + i * Step < port2.Right || MinStepY + i * Step < port2.Bottom; i++)
                    {
                        double X1 = MinStepX + i * Step;
                        double Y1 = MinStepY + i * Step;
                        var P1x = real2VisP(new PointF((float)X1, port2.Top), port2);
                        var P2x = real2VisP(new PointF((float)X1, port2.Bottom), port2);
                        var P1y = real2VisP(new PointF(port2.Left, (float)Y1), port2);
                        var P2y = real2VisP(new PointF(port2.Right, (float)Y1), port2);

                        if (P1x.X < WindowSize.Width)
                        {
                            
                            FrameworkElement l = new Line()
                            {
                                X1 = P1x.X,
                                Y1 = P1x.Y,
                                X2 = P2x.X,
                                Y2 = P2x.Y,
                                Stroke = b,
                                StrokeThickness = GetStroke(X1,Step,BigStep)
                            };
                            result.Add(l);
                            if (Math.Abs((Math.Abs(X1)+Step/5) % BigStep) < Step / 2)
                            {
                                l = new Label()
                                {
                                    Content = $"{X1:0.###}",
                                    Width = lb.Width,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                    Margin = new System.Windows.Thickness((double)(P1x.X - lb.Width / 2d + 7), 0d, 0d, 0d)
                                };
                                result.Add(l);
                            }
                        }

                        if (P1y.Y < WindowSize.Height-hOffset && P1y.Y>lb.Height)
                        {
                            FrameworkElement l = new Line() {
                                X1 = P1y.X,
                                Y1 = P1y.Y,
                                X2 = P2y.X,
                                Y2 = P2y.Y, 
                                Stroke = b, 
                                StrokeThickness = GetStroke(Y1, Step, BigStep)
                            };
                            result.Add(l);
                            if (Math.Abs((Math.Abs(Y1)+Step/5) % BigStep) < Step / 2)
                            {
                                l = new Label()
                                {
                                    Content = $"{Y1:0.###}",
                                    Width = lb.Width,
                                    Height = lb.Height,
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Right,
                                    Margin = new System.Windows.Thickness(0d, (double)(P1y.Y - hOffset - lb.Height + 5), 0d, 0d)
                                };
                                result.Add(l);
                            }
                        }
                    }
                    return result;  
                case DataSet ds: return new ObservableCollection<FrameworkElement>();
                case DataSet[] ads: return new ObservableCollection<FrameworkElement>();
                default: return new ObservableCollection<FrameworkElement>();
            }

            double GetStroke(double X1, double Step,double BigStep)
            {
                switch (X1)
                {
                    case double when Math.Abs(X1) < Step/5:
                        return 1.5d;
                    case double when Math.Abs((Math.Abs(X1)+ Step / 5) % BigStep) < Step/2: 
                        return 0.8d;
                    default: return 0.3d;
                }
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
