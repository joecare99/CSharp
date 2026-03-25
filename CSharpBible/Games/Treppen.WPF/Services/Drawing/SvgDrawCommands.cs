using System.Windows;
using System.Windows.Media;
using Treppen.Export.Services.Interfaces;

namespace Treppen.WPF.Services.Drawing;

// SVG specific draw commands captured from the 3D drawer
public sealed class SvgPolygonCommand : IDrawCommand
{
    public SvgPolygonCommand(Point[] points, Color? fill, Color? stroke, double strokeThickness)
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

    public void Render(object dc) { /* no-op for SVG */ }
}

public sealed class SvgPolyLineCommand : IDrawCommand
{
    public SvgPolyLineCommand(Point[] points, Color stroke, double strokeThickness, bool drawPoints)
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

    public void Render(object dc) { /* no-op for SVG */ }
}

public sealed class SvgDrawCommandFactory : IDrawCommandFactory
{
    public IDrawCommand newPolygonCommand(Point[] points, Color fill, Color? outline, double strokeThickness)
        => new SvgPolygonCommand(points, fill, outline, strokeThickness);

    public IDrawCommand newPolyLineCommand(Point[] points, Color color, double strokeThickness, bool drawPoints)
        => new SvgPolyLineCommand(points, color, strokeThickness, drawPoints);
}
