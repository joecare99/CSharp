using System.Windows;
using System.Windows.Media;
using Treppen.Export.Services.Interfaces;

namespace Treppen.WPF.Services.Drawing;

public class DrawContextDrawingFactory : IDrawCommandFactory
{
    public IDrawCommand newPolygonCommand(Point[] points, Color fill, Color? outline, double v) => new PolygonCommand(points, fill, outline, v);
    public IDrawCommand newPolyLineCommand(Point[] rampPoints, Color color, double v1, bool v2) => new PolyLineCommand(rampPoints, color, v1, v2);
}

public sealed class PolygonCommand : IDrawCommand
{
    public PolygonCommand(Point[] points, Color? fill, Color? stroke, double strokeThickness)
    {
        Points = points;
        Fill = fill;
        Stroke = stroke;
        StrokeThickness = strokeThickness;
    }
    public Point[] Points { get; }
    public Color? Fill { get; }
    public Color? Stroke { get; }
    public double StrokeThickness { get; }

    public void Render(object dc)
    {
        if (Points.Length == 0) return;
        var geometry = new StreamGeometry();
        using (var ctx = geometry.Open())
        {
            ctx.BeginFigure(Points[0], true, true);
            for (int i = 1; i < Points.Length; i++)
            {
                ctx.LineTo(Points[i], true, false);
            }
        }
        geometry.Freeze();
        Brush? fillBrush = Fill.HasValue ? new SolidColorBrush(Fill.Value) : null;
        Brush? strokeBrush = Stroke.HasValue ? new SolidColorBrush(Stroke.Value) : null;
        if (fillBrush is SolidColorBrush fb && !fb.IsFrozen) fb.Freeze();
        if (strokeBrush is SolidColorBrush sb && !sb.IsFrozen) sb.Freeze();
        Pen? pen = strokeBrush != null && StrokeThickness > 0 ? new Pen(strokeBrush, StrokeThickness) : null;
        ((DrawingContext)dc)?.DrawGeometry(fillBrush, pen, geometry);
    }
}

public sealed class PolyLineCommand : IDrawCommand
{
    public PolyLineCommand(Point[] points, Color stroke, double strokeThickness, bool drawPoints)
    {
        Points = points;
        Stroke = stroke;
        StrokeThickness = strokeThickness;
        DrawPoints = drawPoints;
    }
    public Point[] Points { get; }
    public Color Stroke { get; }
    public double StrokeThickness { get; }
    public bool DrawPoints { get; }

    public void Render(object dc)
    {
        if (Points.Length == 0) return;
        var strokeBrush = new SolidColorBrush(Stroke);
        if (!strokeBrush.IsFrozen) strokeBrush.Freeze();
        var pen = new Pen(strokeBrush, StrokeThickness);
        ((DrawingContext)dc)?.DrawEllipse(strokeBrush, null, Points[0], StrokeThickness, StrokeThickness);
        for (int i = 1; i < Points.Length; i++)
        {
            ((DrawingContext)dc)?.DrawLine(pen, Points[i - 1], Points[i]);
            if (DrawPoints)
            {
                ((DrawingContext)dc)?.DrawEllipse(strokeBrush, null, Points[i], StrokeThickness, StrokeThickness);
            }
        }
    }
}
