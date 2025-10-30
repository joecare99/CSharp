// ***********************************************************************
// Assembly         : AA06_Converters_4
// Author           : Mir
// Created: 08-28-2022
//
// Last Modified By : Mir
// Last Modified On : 08-29-2022
// ***********************************************************************
// <copyright file="WindowPortToGridLines.cs" company="JC-Soft">
//     (c) by Joe Care 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using AA06_Converters_4.ViewModels;
using System.Collections.ObjectModel;
using Avalonia.Data.Converters;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Layout;

namespace AA06_Converters_4.View.Converter;

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
    /// The lb (Label-Bereich)
    /// </summary>
    Avalonia.Size lb = new Avalonia.Size(50, 28);

    /// <summary>
    /// Gets or sets the size of the window.
    /// </summary>
    /// <value>The size of the window.</value>
    public Avalonia.Size WindowSize { get; set; } = new Avalonia.Size(600, 600);

    /// <summary>
    /// Real2s the vis.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="visMin">The vis minimum.</param>
    /// <param name="visMax">The vis maximum.</param>
    /// <param name="rMin">The r minimum.</param>
    /// <param name="rMax">The r maximum.</param>
    /// <returns>System.Double.</returns>
    private double Real2Vis(double value, double visMin, double visMax, double rMin, double rMax)
    => visMin + (value - rMin) * (visMax - visMin) / (rMax - rMin);

    /// <summary>
    /// Real2s the vis p.
    /// </summary>
    /// <param name="value">The value.</param>
 /// <param name="port">The port.</param>
    /// <returns>Avalonia.Point.</returns>
    private Avalonia.Point real2VisP(PointF value, RectangleF port) =>
  new Avalonia.Point(
           Real2Vis(value.X, 0, WindowSize.Width - lb.Width, port.Left, port.Right) + lb.Width,
           Real2Vis(value.Y, WindowSize.Height - lb.Height, 0d, port.Top, port.Bottom) + lb.Height);

    /// <summary>
    /// Vis2s the real p.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="port">The port.</param>
    /// <returns>PointF.</returns>
    private PointF vis2RealP(Avalonia.Point value, RectangleF port) =>
     new PointF(
            (float)Real2Vis(value.X - lb.Width, port.Left, port.Right, 0, WindowSize.Width - lb.Width),
 (float)Real2Vis(value.Y - lb.Height, port.Top, port.Bottom, WindowSize.Height - lb.Height, 0d));

    /// <summary>
    /// The real2 vis p
    /// </summary>
    public Func<PointF, RectangleF, Avalonia.Point> Real2VisP;
    
    /// <summary>
    /// The vis2 real p
    /// </summary>
  public Func<Avalonia.Point, RectangleF, PointF> Vis2RealP;
    
    private RectangleF actPort;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowPortToGridLines"/> class.
    /// </summary>
  public WindowPortToGridLines()
    {
   Real2VisP = real2VisP;
        Vis2RealP = vis2RealP;
    }

    /// <summary>
    /// Converts a value.
    /// </summary>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A converted value. If the method returns <see langword="null" />, the valid null value is used.</returns>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        ObservableCollection<Control> result;
        switch (value)
        {
          case SWindowPort c:
           var b = new SolidColorBrush(Colors.Black);

                var hOffset = 0d;
         RectangleF port2; // erweiterter Viewport
port2 = GetAdjustedRect(c);

         double BigStep, Step;
          ComputeGridSteps(port2.Width, out BigStep, out Step);

           double MinStepX = Math.Ceiling(port2.Left / Step) * Step;
    double MinStepY = Math.Ceiling(port2.Top / Step) * Step;

         actPort = port2;
          result = new ObservableCollection<Control>();

  if (c.port.Contains(PointF.Empty))
         {
      var p = Real2VisP(PointF.Empty, port2);
          Ellipse el = new Ellipse() 
      { 
        Height = 7, 
           Width = 7, 
       Margin = new Thickness(p.X - 3, p.Y - 3, 0, 0), 
Stroke = b, 
     StrokeThickness = 0.3d 
    };
    result.Add(el);
              }

            for (var i = 0; MinStepX + i * Step < port2.Right || MinStepY + i * Step < port2.Bottom; i++)
  {
        double X1 = MinStepX + i * Step;
            var P1x = real2VisP(new PointF((float)X1, port2.Top), port2);
      var P2x = real2VisP(new PointF((float)X1, port2.Bottom), port2);
 if (P1x.X < WindowSize.Width)
          {
result.Add(CreateLine(b, GetStroke(X1, Step, BigStep), P1x, P2x));
        
        if (Math.Abs((Math.Abs(X1) + Step / 5) % BigStep) < Step / 2)
    {
  result.Add(CreateLabel(X1, 
               new Thickness((double)(P1x.X - lb.Width / 2d + 7), 0d, 0d, 0d), 
           VerticalAlignment.Bottom, 
           HorizontalAlignment.Center));
    }
      }

          double Y1 = MinStepY + i * Step;
         var P1y = real2VisP(new PointF(port2.Left, (float)Y1), port2);
     var P2y = real2VisP(new PointF(port2.Right, (float)Y1), port2);
           if (P1y.Y < WindowSize.Height - hOffset && P1y.Y > lb.Height)
  {
        result.Add(CreateLine(b, GetStroke(Y1, Step, BigStep), P1y, P2y));      
   if (Math.Abs((Math.Abs(Y1) + Step / 5) % BigStep) < Step / 2)
     {
       result.Add(CreateLabel(Y1, 
    new Thickness(0d, (double)(P1y.Y - hOffset - lb.Height + 5), 0d, 0d), 
     VerticalAlignment.Center, 
  HorizontalAlignment.Right));
             }
     }
    }
     return result;
     
          case DataSet ds:
        result = new ObservableCollection<Control>();
                if (ds.Datapoints?.Length > 1)
     {
           for (int i = 0; i < ds.Datapoints.Length - 1; i++)
      {
            var P1 = real2VisP(ds.Datapoints[i], actPort);
 var P2 = real2VisP(ds.Datapoints[i + 1], actPort);
 result.Add(CreateLine(ds.Pen.Brush, ds.Pen.Thickness, P1, P2));
                  }
        }
            return result;
          
 case DataSet[] ads: 
          return new ObservableCollection<Control>();
  
            case ArrowList al: 
         result = new ObservableCollection<Control>();
     foreach (var sh in al)
                {
           var P1 = real2VisP(sh.Start, actPort);
             var P2 = real2VisP(sh.End, actPort);
  foreach (var el in CreateArrow(al.Pen?.Brush, al.Pen?.Thickness ?? 1, P1, P2))
     result.Add(el);
     }
         return result;
                
    case CircleList cl:
       result = new ObservableCollection<Control>();
          foreach (var sh in cl)
          {
var P1 = real2VisP(sh.Center, actPort);
           var r = Real2VisP(PointF.Add(sh.Center, new SizeF((float)sh.Radius, 0)), actPort).X - P1.X;
      result.Add(CreateCircle(cl.Pen?.Brush, cl.Pen?.Thickness ?? 1, P1, (float)r));
         }
    return result;
     
            case PolynomeList pl:
  result = new ObservableCollection<Control>();
           foreach (var sh in pl)
    {
          var p = new List<Avalonia.Point>();
             foreach (var pnt in sh.Points)
             p.Add(real2VisP(pnt, actPort));
        result.Add(CreatePolynome(pl.Pen?.Brush, pl.Pen?.Thickness ?? 1, p));
     }
      return result;
     
    default: 
     return new ObservableCollection<Control>();
        }

        double GetStroke(double X1, double Step, double BigStep)
        {
    switch (X1)
       {
     case double when Math.Abs(X1) < Step / 5:
     return 1.5d;
         case double when Math.Abs((Math.Abs(X1) + Step / 5) % BigStep) < Step / 2:
       return 0.8d;
    default: 
return 0.3d;
       }
        }
    }

    /// <summary>
    /// Gets the adjusted rect.
    /// </summary>
    /// <param name="c">The c.</param>
    /// <returns>RectangleF.</returns>
    public RectangleF GetAdjustedRect(SWindowPort c)
    {
        RectangleF port2;
        if (Math.Abs(WindowSize.Width * c.port.Height) < Math.Abs(WindowSize.Height * c.port.Width))
       port2 = new RectangleF(
      c.port.Left, 
 (float)(c.port.Top + c.port.Height * 0.5f - (WindowSize.Height * 0.5f) * c.port.Width / (float)WindowSize.Width), 
     c.port.Width, 
    (float)(c.WindowSize.Height * c.port.Width / WindowSize.Width));
        else
        port2 = new RectangleF(
        (float)(c.port.Left + c.port.Width * 0.5 - (WindowSize.Width / 2) * c.port.Height / WindowSize.Height), 
    c.port.Top, 
      (float)(WindowSize.Width * c.port.Height / WindowSize.Height), 
        c.port.Height);
        return port2;
    }

    /// <summary>
    /// Creates a line.
    /// </summary>
    /// <param name="b">The brush.</param>
    /// <param name="value">The stroke thickness.</param>
    /// <param name="P1">The start point.</param>
    /// <param name="P2">The end point.</param>
    /// <returns>Control.</returns>
    Control CreateLine(IBrush? b, double value, Avalonia.Point P1, Avalonia.Point P2)
    {
        return new Line()
{
  StartPoint = P1,
   EndPoint = P2,
  Stroke = b ?? Brushes.Black,
  StrokeThickness = value
        };
    }

    /// <summary>
    /// Creates a circle.
    /// </summary>
    /// <param name="b">The brush.</param>
    /// <param name="value">The stroke thickness.</param>
    /// <param name="P1">The center point.</param>
    /// <param name="r">The radius.</param>
    /// <returns>Control.</returns>
    Control CreateCircle(IBrush? b, double value, Avalonia.Point P1, float r)
    {
        return new Ellipse()
        {
     Margin = new Thickness(P1.X - r, P1.Y - r, 0, 0),
            Height = r * 2,
       Width = r * 2,
   Stroke = b ?? Brushes.Black,
            StrokeThickness = value
 };
    }

    /// <summary>
 /// Creates a polynome/polygon.
    /// </summary>
    /// <param name="b">The brush.</param>
    /// <param name="value">The stroke thickness.</param>
    /// <param name="Pts">The points.</param>
    /// <returns>Control.</returns>
    Control CreatePolynome(IBrush? b, double value, List<Avalonia.Point> Pts)
    {
   return new Polygon()
        {
 Points = new Points(Pts),
            Stroke = b ?? Brushes.Black,
            StrokeThickness = value
        };
    }

    /// <summary>
    /// Creates an arrow (line with arrowhead).
    /// </summary>
    /// <param name="b">The brush.</param>
    /// <param name="value">The stroke thickness.</param>
    /// <param name="P1">The start point.</param>
    /// <param name="P2">The end point.</param>
  /// <returns>Enumerable of controls.</returns>
    IEnumerable<Control> CreateArrow(IBrush? b, double value, Avalonia.Point P1, Avalonia.Point P2)
    {
        // Hauptlinie
     yield return new Line()
    {
 StartPoint = P1,
          EndPoint = P2,
            Stroke = b ?? Brushes.Black,
       StrokeThickness = value,     
};
        
        // Pfeilspitze
        System.Numerics.Vector2 v = new((float)(P2.X - P1.X), (float)(P2.Y - P1.Y));
        var l = v.Length();
        if (l > 0)
        {
        var ve = v * (1 / l);
       yield return new Line()
{
        StartPoint = new Avalonia.Point(P2.X - ve.X * (value * 2), P2.Y - ve.Y * (value * 2)),
                EndPoint = new Avalonia.Point(P2.X - ve.X * value, P2.Y - ve.Y * value),
       Stroke = b ?? Brushes.Black,
    StrokeThickness = value * 5,
     StrokeLineCap = PenLineCap.Round // Avalonia hat kein Triangle, Round ist ähnlich
 };
        }
    }

    /// <summary>
    /// Creates a label.
    /// </summary>
    /// <param name="Y1">The value to display.</param>
    /// <param name="margin">The margin.</param>
    /// <param name="va">The vertical alignment.</param>
    /// <param name="ha">The horizontal alignment.</param>
    /// <returns>Control.</returns>
 Control CreateLabel(double Y1, Thickness margin, VerticalAlignment va, HorizontalAlignment ha)
 {
        return new TextBlock()
        {
    Text = $"{Y1:0.###}",
 Width = lb.Width,
      Height = lb.Height,
            VerticalAlignment = va,
            HorizontalAlignment = ha,
            Margin = margin
        };
    }

    /// <summary>
    /// Computes the grid steps.
    /// </summary>
    /// <param name="vpWidth">Width of the viewport.</param>
    /// <param name="BigStep">The big step.</param>
    /// <param name="Step">The step.</param>
    private void ComputeGridSteps(double vpWidth, out double BigStep, out double Step)
    {
        BigStep = Math.Pow(10, Math.Floor(Math.Log10(vpWidth * AvgGrdPixel * 10 / WindowSize.Width)));
        while ((BigStep / vpWidth * WindowSize.Width) < AvgGrdPixel * 3 / 1.5)
      BigStep *= 2d;
        while ((BigStep / vpWidth * WindowSize.Width) > AvgGrdPixel * 4.5)
      BigStep *= 0.5d;
        switch ((BigStep / vpWidth * WindowSize.Width) / AvgGrdPixel)
        {
      case double d when d > 3.5: 
    Step = BigStep / 10; 
break;
    case double d when d < 2.5:
     Step = BigStep / 4; 
   break;
     default: 
         Step = BigStep / 5; 
            break;
        }
    }

    /// <summary>
    /// Converts a value back (not implemented).
    /// </summary>
    /// <param name="value">The value that is produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
/// <returns>A converted value.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
  throw new NotImplementedException();
    }
}
