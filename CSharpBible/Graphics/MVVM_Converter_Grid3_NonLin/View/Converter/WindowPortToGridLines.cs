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
using MVVM_Converter_Grid3_NonLin.ViewModel;
using System.Collections.ObjectModel;
using System.Windows;
using BaseLib.Helper;
using System.Numerics;

namespace MVVM_Converter_Grid3_NonLin.View.Converter
{
    public class WindowPortToGridLines : IValueConverter
    {
        #region Property
        #region private Property
        const int AvgGrdPixel = 40;
        private System.Windows.Size lb = new System.Windows.Size(50, 28);
        private System.Windows.Size _windowSize = new System.Windows.Size(600, 600);
        private RectangleF port2; // erweiterter Viewport
        private RectangleF cport;
        private PointF[]? EvalP=default;
        private Vector2[]? EvaldP=default;
        private Func<PointF, PointF> _Real2VisP2 = ZeroTransform();
        #endregion

        public Func<PointF, RectangleF, System.Windows.Point> Real2VisP;
        public Func<PointF, PointF> Real2VisP2 { get => _Real2VisP2; set =>Property.SetProperty(ref _Real2VisP2, value,EvalFunction); }

        private void EvalFunction(string arg1, Func<PointF, PointF> arg2, Func<PointF, PointF> arg3)
        {
            EvalP = new PointF[600];
            EvaldP = new Vector2[600];
            for (int y = -10;y<10;y++)
                for (int x = -10;x<20;x++)
                {
                    var (xx, yy) = (x * 0.1f, y * 0.1f);
                    var p = EvalP[x + 10 + (y + 10) * 30] =  arg3.Invoke(new PointF(xx, yy));
                    var dp = (arg3.Invoke(new PointF(xx+0.01f, yy+0.01f)).ToVector2()-p.ToVector2())*100f;
                    EvaldP[x + 10 + (y + 10) * 30] = dp;
                }
        }

        public Func<System.Windows.Point, RectangleF, PointF> Vis2RealP;
        private bool IsLinear=>Func<PointF, PointF>.Equals((object)ZeroTransform,_Real2VisP2);

        public System.Windows.Size WindowSize { get => _windowSize; set => Property.SetProperty(ref _windowSize,value,WindowSizeChanged); }
        #endregion

        #region Methods
        public WindowPortToGridLines()
        {
            Real2VisP = real2VisP;
        }
        private static Func<PointF, PointF> ZeroTransform()
        {
            return (p) => p;
        }

        private void WindowSizeChanged(string arg1, System.Windows.Size arg2, System.Windows.Size arg3)
        {
            if (Math.Abs(WindowSize.Width * cport.Height) < Math.Abs(WindowSize.Height * cport.Width))
                port2 = new RectangleF(cport.Left, (float)(cport.Top + cport.Height * 0.5f - (WindowSize.Height * 0.5f) * cport.Width / (float)WindowSize.Width), cport.Width, (float)(WindowSize.Height * cport.Width / WindowSize.Width));
            else
                port2 = new RectangleF((float)(cport.Left + cport.Width * 0.5 - (WindowSize.Width / 2) * cport.Height / WindowSize.Height), cport.Top, (float)(WindowSize.Width * cport.Height / WindowSize.Height), cport.Height);
        }

        private double Real2Vis(double value, double visMin, double visMax, double rMin, double rMax)
            => visMin + (value - rMin) * (visMax - visMin) / (rMax - rMin);

        private System.Windows.Point real2VisP(PointF value, RectangleF port)
        {
            var p = _Real2VisP2(value);
            return new System.Windows.Point(
                Real2Vis(p.X, 0, WindowSize.Width - lb.Width, port.Left, port.Right) + lb.Width,
                Real2Vis(p.Y, WindowSize.Height - lb.Height, 0d, port.Top, port.Bottom) + lb.Height);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case SWindowPort c:
                    var b = new SolidColorBrush(Colors.Black);

                    var hOffset = 0d;
                    if (c.port != cport)
                    { 
                        if (Math.Abs(WindowSize.Width * c.port.Height) < Math.Abs(WindowSize.Height * c.port.Width))
                            port2 = new RectangleF(c.port.Left, (float)(c.port.Top + c.port.Height * 0.5f - (_windowSize.Height * 0.5f) * c.port.Width / (float)WindowSize.Width), c.port.Width, (float)(WindowSize.Height * c.port.Width / WindowSize.Width));
                        else
                            port2 = new RectangleF((float)(c.port.Left + c.port.Width * 0.5 - (WindowSize.Width / 2) * c.port.Height / WindowSize.Height), c.port.Top, (float)(WindowSize.Width * c.port.Height / WindowSize.Height), c.port.Height);
                        cport = c.port;
                    }

                    double BigStep, Step;
                    ComputeGridSteps(port2.Width, out BigStep, out Step);
                    
                    double MinStepX = Math.Ceiling(port2.Left / Step) * Step;
                    double MinStepY = Math.Ceiling(port2.Top / Step) * Step;

                    ObservableCollection<FrameworkElement> result = new ObservableCollection<FrameworkElement>();

                    if (c.port.Contains(PointF.Empty))
                    {
                        var p = Real2VisP(PointF.Empty, port2);
                        Ellipse el = new Ellipse() { Height = 7, Width = 7, Margin = new Thickness(p.X - 3, p.Y - 3, 0, 0), Stroke = b, StrokeThickness = 0.3d };
                        result.Add(el);
                    }

                    //                    var MaxSize = WindowSize.Width > (WindowSize.Height- hOffset) *1.5 ? (WindowSize.Width-lb.Width) / 1.5 : (WindowSize.Height-lb.Height- hOffset) ;

                    for (var i = 0; MinStepX + i * Step < port2.Right || MinStepY + i * Step < port2.Bottom; i++)
                    {
                        double X1 = MinStepX + i * Step;
                        var P1x = Real2VisP(new PointF((float)X1, port2.Top), port2);
                        var P2x = Real2VisP(new PointF((float)X1, port2.Bottom), port2);

                        if (P1x.X < WindowSize.Width)
                        {
                            FrameworkElement l;
                            if (IsLinear)
                                 l = CreateLine(b, GetStroke(X1, Step, BigStep), P1x, P2x);
                            else
                            {
                                PointCollection points = new PointCollection();
                                for (var j = 0; MinStepY + j * Step < port2.Bottom; j++)
                                    points.Add(Real2VisP(new PointF((float)X1,(float)(MinStepY+j*Step)),port2));
                                l = CreatePolyLine(b, GetStroke(X1, Step, BigStep), points);
                            }

                            result.Add(l);
                            if (Math.Abs((Math.Abs(X1) + Step / 5) % BigStep) < Step / 2)
                            {
                                l = CreateLabel(X1, new System.Windows.Thickness((double)(P1x.X - lb.Width / 2d + 7), 0d, 0d, 0d), VerticalAlignment.Bottom, HorizontalAlignment.Center);
                                result.Add(l);
                            }
                        }

                        double Y1 = MinStepY + i * Step;
                        var P1y = Real2VisP(new PointF(port2.Left, (float)Y1), port2);
                        var P2y = Real2VisP(new PointF(port2.Right, (float)Y1), port2);
                        if (P1y.Y < WindowSize.Height - hOffset && P1y.Y > lb.Height)
                        {
                            FrameworkElement l;
                            if (IsLinear)
                                l = CreateLine(b, GetStroke(Y1, Step, BigStep), P1y, P2y);
                            else
                            {
                                PointCollection points = new PointCollection();
                                for (var j = 0; MinStepX + j * Step < port2.Right; j++)
                                    points.Add(Real2VisP(new PointF((float)(MinStepX + j * Step), (float)Y1 ), port2));
                                l = CreatePolyLine(b, GetStroke(X1, Step, BigStep), points);
                            }
                            result.Add(l);
                            if (Math.Abs((Math.Abs(Y1) + Step / 5) % BigStep) < Step / 2)
                            {
                                l = CreateLabel(Y1, new System.Windows.Thickness(0d, (double)(P1y.Y - hOffset - lb.Height + 5), 0d, 0d), VerticalAlignment.Center, HorizontalAlignment.Right);
                                result.Add(l);
                            }
                        }
                    }
                    return result;
                case DataSet ds: return new ObservableCollection<FrameworkElement>();
                case DataSet[] ads: return new ObservableCollection<FrameworkElement>();
                default: return new ObservableCollection<FrameworkElement>();
            }

            double GetStroke(double X1, double Step, double BigStep)
            {
                switch (X1)
                {
                    case double when Math.Abs(X1) < Step / 5:
                        return 1.5d;
                    case double when Math.Abs((Math.Abs(X1) + Step / 5) % BigStep) < Step / 2:
                        return 0.8d;
                    default: return 0.3d;
                }
            }

        }

        FrameworkElement CreateLine(SolidColorBrush b, double value, System.Windows.Point P1, System.Windows.Point P2)
        {
            return new Line()
            {
                X1 = P1.X,
                Y1 = P1.Y,
                X2 = P2.X,
                Y2 = P2.Y,
                Stroke = b,
                StrokeThickness = value
            };
        }

        FrameworkElement CreatePolyLine(SolidColorBrush b, double StrThk, PointCollection Pl)
        {
            return new Polyline()
            {
                Points = Pl,
                Stroke = b,
                StrokeThickness = StrThk
            };
        }


        FrameworkElement CreateLabel(double Y1, Thickness margin, VerticalAlignment va, HorizontalAlignment ha)
        {
            return new Label()
            {
                Content = $"{Y1:0.###}",
                Width = lb.Width,
                Height = lb.Height,
                VerticalAlignment = va,
                HorizontalAlignment = ha,
                Margin = margin
            };
        }

        private void ComputeGridSteps(double vpWidth, out double BigStep, out double Step)
        {
            BigStep = Math.Pow(10, Math.Floor(Math.Log10(vpWidth * AvgGrdPixel * 10 / WindowSize.Width)));
            while ((BigStep / vpWidth * WindowSize.Width) < AvgGrdPixel * 3 / 1.5)
                BigStep *= 2d;
            while ((BigStep / vpWidth * WindowSize.Width) > AvgGrdPixel * 4.5)
                BigStep *= 0.5d;
            switch ((BigStep / vpWidth * WindowSize.Width) / AvgGrdPixel)
            {
                case double d when d > 3.5: Step = BigStep / 10; break;
                case double d when d < 2.5:
                    Step = BigStep / 4; break;
                default: Step = BigStep / 5; break;
            }
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
