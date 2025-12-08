using AsteroidsModern.Engine.Abstractions;
using System.Numerics;
using System.Windows;
using SWM = System.Windows.Media;

namespace AsteroidsModern.UI;

public sealed class WpfRenderContext : IRenderContext
{
    private readonly SWM.DrawingContext _dc;

    public WpfRenderContext(SWM.DrawingContext dc)
    {
        _dc = dc;
    }

    public void Clear(Color color)
    {
        // handled by background brush in control
    }

    public void DrawPolygon(Vector2[] points, Color color, float thickness = 1)
    {
        var pen = new SWM.Pen(new SWM.SolidColorBrush(SWM.Color.FromArgb(color.A, color.R, color.G, color.B)), thickness);
        var geom = new SWM.StreamGeometry();
        using var ctx = geom.Open();
        ctx.BeginFigure(new System.Windows.Point(points[0].X, points[0].Y), false, true);
        for (int i = 1; i < points.Length; i++)
            ctx.LineTo(new System.Windows.Point(points[i].X, points[i].Y), true, false);
        _dc.DrawGeometry(null, pen, geom);
    }

    public void DrawCircle(Vector2 center, float radius, Color color, float thickness = 1, int segments = 24)
    {
        var pen = new SWM.Pen(new SWM.SolidColorBrush(SWM.Color.FromArgb(color.A, color.R, color.G, color.B)), thickness);
        _dc.DrawEllipse(null, pen, new System.Windows.Point(center.X, center.Y), radius, radius);
    }

    public void DrawText(string text, Vector2 position, Color color, float fontSize = 12)
    {
        _dc.DrawText(
            new SWM.FormattedText(
                text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new SWM.Typeface("Consolas"),
                fontSize,
                new SWM.SolidColorBrush(SWM.Color.FromArgb(color.A, color.R, color.G, color.B)),
                1.0),
            new System.Windows.Point(position.X, position.Y));
    }

    public void DrawPixel(Vector2 center, Color color)
    {
        _dc.DrawRectangle(new SWM.SolidColorBrush(SWM.Color.FromArgb(color.A, color.R, color.G, color.B)), null, new Rect(center.X, center.Y, 1, 1));
    }
}
