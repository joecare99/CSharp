using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Genealogy.Statistics;

namespace Genealogy.Avalonia.Views;

/// <summary>
/// Draws the normalized genealogy yearly age timeline on shared chart coordinates.
/// </summary>
public sealed class GenealogyStatisticsAgeDistributionChart : Control
{
    public static readonly StyledProperty<GenealogyStatisticsAgeDistributionChartModel?> ChartProperty =
        AvaloniaProperty.Register<GenealogyStatisticsAgeDistributionChart, GenealogyStatisticsAgeDistributionChartModel?>(nameof(Chart));

    private static readonly Pen AxisPen = new(Brushes.Gray, 1);
    private static readonly Pen AgePen = new(Brushes.DodgerBlue, 2);
    private static readonly Pen GenerationGapPen = new(Brushes.ForestGreen, 2);
    private static readonly Pen DeathAgePen = new(Brushes.IndianRed, 2);
    private static readonly Pen GridPen = new(Brushes.LightGray, 1);
    private static readonly Pen CenturyGridPen = new(Brushes.Gray, 1.5);

    /// <summary>Gets or sets the normalized age timeline data.</summary>
    public GenealogyStatisticsAgeDistributionChartModel? Chart
    {
        get => GetValue(ChartProperty);
        set => SetValue(ChartProperty, value);
    }

    static GenealogyStatisticsAgeDistributionChart() => AffectsRender<GenealogyStatisticsAgeDistributionChart>(ChartProperty);

    /// <inheritdoc />
    public override void Render(DrawingContext context)
    {
        base.Render(context);
        var chart = Chart;
        var bounds = Bounds;
        if (chart is null || chart.Points.Count == 0 || bounds.Width < 40 || bounds.Height < 40)
        {
            return;
        }

        const double left = 55;
        const double right = 55;
        const double top = 22;
        const double bottom = 42;
        var plotLeft = left;
        var plotRight = bounds.Width - right;
        var plotTop = top;
        var plotBottom = bounds.Height - bottom;
        var plotWidth = plotRight - plotLeft;
        var plotHeight = plotBottom - plotTop;

        foreach (var tick in chart.TimeAxisTicks)
        {
            var x = plotLeft + (tick.Value - chart.FirstYear) / Math.Max(1.0, chart.LastYear - chart.FirstYear) * plotWidth;
            context.DrawLine(tick.IsCentury ? CenturyGridPen : GridPen, new Point(x, plotTop), new Point(x, plotBottom));
            DrawLabel(context, tick.Label, new Point(x - 18, plotBottom + 5), 11);
        }

        foreach (var tick in chart.YAxisTicks)
        {
            var y = plotBottom - tick.Value / Math.Max(1.0, chart.YAxisMaximum) * plotHeight;
            context.DrawLine(tick.IsMajor ? GridPen : new Pen(Brushes.Gainsboro, 0.5), new Point(plotLeft, y), new Point(plotRight, y));
            if (tick.IsMajor)
            {
                DrawLabel(context, tick.Label, new Point(4, y - 7), 11);
            }
        }

        context.DrawLine(AxisPen, new Point(plotLeft, plotTop), new Point(plotLeft, plotBottom));
        context.DrawLine(AxisPen, new Point(plotLeft, plotBottom), new Point(plotRight, plotBottom));
        DrawSeries(context, chart, plotLeft, plotBottom, plotWidth, plotHeight, point => point.Age, AgePen);
        DrawSeries(context, chart, plotLeft, plotBottom, plotWidth, plotHeight, point => point.GenerationGap, GenerationGapPen);
        DrawSeries(context, chart, plotLeft, plotBottom, plotWidth, plotHeight, point => point.DeathAge, DeathAgePen);
        DrawLabel(context, "Jahr", new Point(plotRight - 25, plotBottom + 20), 11);
        DrawLabel(context, "Werte", new Point(2, 2), 11);
    }

    private static void DrawLabel(DrawingContext context, string text, Point origin, double fontSize)
    {
        var formatted = new FormattedText(
            text,
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            Typeface.Default,
            fontSize,
            Brushes.Black);
        context.DrawText(formatted, origin);
    }

    private static void DrawSeries(
        DrawingContext context,
        GenealogyStatisticsAgeDistributionChartModel chart,
        double left,
        double bottom,
        double width,
        double height,
        Func<GenealogyStatisticsAgeDistributionChartPoint, double> valueSelector,
        Pen pen)
    {
        if (chart.Points.Count == 1)
        {
            var point = chart.Points[0];
            var center = new Point(left + width / 2, bottom - Math.Clamp(valueSelector(point), 0, 1) * height);
            context.DrawEllipse(pen.Brush, pen, center, 3, 3);
            return;
        }

        for (var index = 1; index < chart.Points.Count; index++)
        {
            var previous = chart.Points[index - 1];
            var current = chart.Points[index];
            var previousX = left + (previous.Year - chart.FirstYear) / Math.Max(1.0, chart.LastYear - chart.FirstYear) * width;
            var currentX = left + (current.Year - chart.FirstYear) / Math.Max(1.0, chart.LastYear - chart.FirstYear) * width;
            var previousY = bottom - Math.Clamp(valueSelector(previous), 0, 1) * height;
            var currentY = bottom - Math.Clamp(valueSelector(current), 0, 1) * height;
            context.DrawLine(pen, new Point(previousX, previousY), new Point(currentX, currentY));
        }
    }
}
