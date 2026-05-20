using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TraceAnalysis.Widgets.Wpf.ViewModels;

namespace TraceAnalysis.Widgets.Wpf.Controls;

/// <summary>
/// Interaction logic for the trace chart shell widget.
/// </summary>
public partial class TraceChartControl : UserControl
{
    /// <summary>
    /// Initializes a new instance of <see cref="TraceChartControl"/>.
    /// </summary>
    public TraceChartControl()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
        SizeChanged += (_, _) => RenderChart();
        Loaded += (_, _) => RenderChart();
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is TraceChartViewModel oldViewModel)
        {
            oldViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            oldViewModel.Series.CollectionChanged -= OnSeriesCollectionChanged;
            foreach (var series in oldViewModel.Series)
                series.PropertyChanged -= OnSeriesPropertyChanged;
        }

        if (e.NewValue is TraceChartViewModel newViewModel)
        {
            newViewModel.PropertyChanged += OnViewModelPropertyChanged;
            newViewModel.Series.CollectionChanged += OnSeriesCollectionChanged;
            foreach (var series in newViewModel.Series)
                series.PropertyChanged += OnSeriesPropertyChanged;
        }

        RenderChart();
    }

    private void OnSeriesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            foreach (ChartSeriesViewModel series in e.OldItems)
                series.PropertyChanged -= OnSeriesPropertyChanged;
        }

        if (e.NewItems != null)
        {
            foreach (ChartSeriesViewModel series in e.NewItems)
                series.PropertyChanged += OnSeriesPropertyChanged;
        }

        RenderChart();
    }

    private void OnSeriesPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ChartSeriesViewModel.IsVisible))
            RenderChart();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(TraceChartViewModel.ViewStart):
            case nameof(TraceChartViewModel.ViewEnd):
            case nameof(TraceChartViewModel.CursorA):
            case nameof(TraceChartViewModel.CursorB):
            case nameof(TraceChartViewModel.VisibleSeries):
            case nameof(TraceChartViewModel.VisibleMinimumX):
            case nameof(TraceChartViewModel.VisibleMaximumX):
            case nameof(TraceChartViewModel.VisibleMinimumY):
            case nameof(TraceChartViewModel.VisibleMaximumY):
                RenderChart();
                break;
        }
    }

    private void RenderChart()
    {
        plotCanvas.Children.Clear();

        if (DataContext is not TraceChartViewModel viewModel)
            return;

        var width = Math.Max(0d, plotCanvas.ActualWidth);
        var height = Math.Max(0d, plotCanvas.ActualHeight);
        if (width < 2d || height < 2d)
            return;

        DrawGrid(width, height);

        var visibleSeries = viewModel.VisibleSeries;
        if (visibleSeries.Count == 0)
            return;

        var xMin = Math.Min(viewModel.ViewStart, viewModel.ViewEnd);
        var xMax = Math.Max(viewModel.ViewStart, viewModel.ViewEnd);
        if (Math.Abs(xMax - xMin) < double.Epsilon)
            xMax = xMin + 1d;

        var yMin = viewModel.VisibleMinimumY;
        var yMax = viewModel.VisibleMaximumY;
        if (Math.Abs(yMax - yMin) < double.Epsilon)
            yMax = yMin + 1d;

        var palette = new[]
        {
            Color.FromRgb(0x8B, 0xC6, 0xFF),
            Color.FromRgb(0xFF, 0xB0, 0x6A),
            Color.FromRgb(0x88, 0xE1, 0x9F),
            Color.FromRgb(0xDE, 0x9E, 0xFF),
            Color.FromRgb(0xFF, 0x92, 0xB2)
        };

        for (var i = 0; i < visibleSeries.Count; i++)
        {
            var series = visibleSeries[i];
            var windowPoints = series.Points
                .Where(point => point.Time >= xMin && point.Time <= xMax)
                .ToArray();
            if (windowPoints.Length < 2)
                continue;

            var polyline = new Polyline
            {
                Stroke = new SolidColorBrush(palette[i % palette.Length]),
                StrokeThickness = 1.5
            };

            foreach (var point in windowPoints)
            {
                polyline.Points.Add(new Point(
                    MapX(point.Time, xMin, xMax, width),
                    MapY(point.Value, yMin, yMax, height)));
            }

            plotCanvas.Children.Add(polyline);
        }

        plotCanvas.Children.Add(CreateCursor(viewModel.CursorA, xMin, xMax, width, height, Color.FromRgb(0xE4, 0xB4, 0x00)));
        plotCanvas.Children.Add(CreateCursor(viewModel.CursorB, xMin, xMax, width, height, Color.FromRgb(0xFF, 0x7F, 0x50)));
    }

    private void DrawGrid(double width, double height)
    {
        for (var i = 1; i < 4; i++)
        {
            var x = width * i / 4d;
            plotCanvas.Children.Add(new Line
            {
                X1 = x,
                X2 = x,
                Y1 = 0d,
                Y2 = height,
                Stroke = new SolidColorBrush(Color.FromArgb(40, 255, 255, 255)),
                StrokeThickness = 1d
            });
        }

        for (var i = 1; i < 4; i++)
        {
            var y = height * i / 4d;
            plotCanvas.Children.Add(new Line
            {
                X1 = 0d,
                X2 = width,
                Y1 = y,
                Y2 = y,
                Stroke = new SolidColorBrush(Color.FromArgb(40, 255, 255, 255)),
                StrokeThickness = 1d
            });
        }
    }

    private static Line CreateCursor(double xValue, double xMin, double xMax, double width, double height, Color color)
    {
        var x = MapX(xValue, xMin, xMax, width);
        return new Line
        {
            X1 = x,
            X2 = x,
            Y1 = 0d,
            Y2 = height,
            Stroke = new SolidColorBrush(color),
            StrokeThickness = 1d
        };
    }

    private static double MapX(double value, double min, double max, double width)
    {
        return ((value - min) / (max - min)) * width;
    }

    private static double MapY(double value, double min, double max, double height)
    {
        return height - (((value - min) / (max - min)) * height);
    }
}
