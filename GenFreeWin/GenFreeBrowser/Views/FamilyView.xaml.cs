using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GenFreeBrowser.Views.Models;

namespace GenFreeBrowser.Views
{
    public partial class FamilyView : UserControl
    {
        private const double CardWidth = 150;
        private const double CardHeight = 80;
        private const double HSpacing = 30;
        private const double VSpacing = 60;

        private readonly Canvas _lineCanvas = new();

        public FamilyView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            DataContextChanged += OnDataContext;
            SizeChanged += OnLoaded;

            _lineCanvas.IsHitTestVisible = false;
            Panel.SetZIndex(_lineCanvas, 0);
            GraphCanvas.Children.Insert(0, _lineCanvas);
        }

        private void OnDataContext(object sender, DependencyPropertyChangedEventArgs e)
        {
            OnLoaded(sender, new RoutedEventArgs());
        }

        private void OnLoaded(object? sender, RoutedEventArgs e)
        {
            var graph = ResolveGraph(DataContext);
            if (graph is null) return;
            LayoutGraph(graph);
            DrawConnectors(graph);
        }

        private static FamilyGraph? ResolveGraph(object? dc)
        {
            if (dc is FamilyGraph g) return g;
            if (dc is null) return null;
            // Try property named Graph
            var prop = dc.GetType().GetProperty("Graph", BindingFlags.Public | BindingFlags.Instance);
            if (prop?.GetValue(dc) is FamilyGraph gg) return gg;
            return null;
        }

        private void LayoutGraph(FamilyGraph graph)
        {
            var gens = graph.Nodes
                .GroupBy(n => n.Generation)
                .OrderBy(g => g.Key)
                .ToList();

            double y = 0;
            foreach (var gen in gens)
            {
                var items = gen.OrderBy(n => n.Order).ToList();
                double x = 0;
                foreach (var node in items)
                {
                    node.X = x;
                    node.Y = y;
                    x += CardWidth + HSpacing;
                }
                y += CardHeight + VSpacing;
            }
        }

        private void DrawConnectors(FamilyGraph graph)
        {
            _lineCanvas.Children.Clear();
            // Partner lines
            foreach (var p in graph.Partners)
            {
                var a = graph.Nodes.First(n => n.Id == p.A);
                var b = graph.Nodes.First(n => n.Id == p.B);
                if (a.X > b.X) (a, b) = (b, a);
                _lineCanvas.Children.Add(MakeLine(a.X + CardWidth, a.Y + CardHeight / 2.0, b.X, b.Y + CardHeight / 2.0, Color.FromRgb(165, 184, 122)));
            }

            // Parent-child connectors
            foreach (var pc in graph.ParentChild)
            {
                var a = graph.Nodes.First(n => n.Id == pc.ParentA);
                var b = graph.Nodes.First(n => n.Id == pc.ParentB);
                var c = graph.Nodes.First(n => n.Id == pc.Child);
                if (a.X > b.X) (a, b) = (b, a);
                var y = (a.Y + CardHeight / 2.0 + b.Y + CardHeight / 2.0) / 2.0;
                var x1 = a.X + CardWidth;
                var x2 = b.X;
                var xm = (x1 + x2) / 2.0;
                var cx = c.X + CardWidth / 2.0;
                var cy = c.Y;
                var ym = (c.Y+ a.Y + CardHeight) / 2.0;

                _lineCanvas.Children.Add(MakeLine(x1, y, x2, y, Color.FromRgb(154, 163, 173)));
                _lineCanvas.Children.Add(MakeLine(xm, y, xm, ym, Color.FromRgb(154, 163, 173)));
                _lineCanvas.Children.Add(MakeLine(xm, ym, cx, ym, Color.FromRgb(154, 163, 173)));
                _lineCanvas.Children.Add(MakeLine(cx, ym, cx, cy, Color.FromRgb(154, 163, 173)));
            }
        }

        private static Line MakeLine(double x1, double y1, double x2, double y2, Color color)
        {
            return new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = new SolidColorBrush(color),
                StrokeThickness = 2,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round
            };
        }
    }
}
