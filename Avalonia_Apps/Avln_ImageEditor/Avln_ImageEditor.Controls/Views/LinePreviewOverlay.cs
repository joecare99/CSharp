using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Avln_ImageEditor.Controls.Views;

/// <summary>Draws a non-interactive position guide, not a pixel-accurate stroke.</summary>
public sealed class LinePreviewOverlay : Control
{
    public static readonly StyledProperty<double> ZoomProperty =
        AvaloniaProperty.Register<LinePreviewOverlay, double>(nameof(Zoom), 1);

    static LinePreviewOverlay() => AffectsRender<LinePreviewOverlay>(ZoomProperty);

    public double Zoom
    {
        get => GetValue(ZoomProperty);
        set => SetValue(ZoomProperty, value);
    }

    public Point Start { get; private set; }
    public Point End { get; private set; }
    public bool HasPreview { get; private set; }

    internal void SetPreview(Point start, Point end, PixelSize imageSize)
    {
        Start = new Point(Math.Floor(start.X) + .5, Math.Floor(start.Y) + .5);
        var target = new Point(Math.Floor(end.X) + .5, Math.Floor(end.Y) + .5);
        double dx = target.X - Start.X, dy = target.Y - Start.Y, fraction = 1;
        if (dx > 0) fraction = Math.Min(fraction, (imageSize.Width - Start.X) / dx);
        else if (dx < 0) fraction = Math.Min(fraction, -Start.X / dx);
        if (dy > 0) fraction = Math.Min(fraction, (imageSize.Height - Start.Y) / dy);
        else if (dy < 0) fraction = Math.Min(fraction, -Start.Y / dy);
        End = new Point(Start.X + fraction * dx, Start.Y + fraction * dy);
        HasPreview = true;
        InvalidateVisual();
    }

    internal void ClearPreview()
    {
        if (!HasPreview) return;
        HasPreview = false;
        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (!HasPreview || !double.IsFinite(Zoom) || Zoom <= 0) return;
        using (context.PushClip(new Rect(Bounds.Size)))
        {
            var outer = new Pen(Brushes.Black, 4 / Zoom);
            var inner = new Pen(Brushes.Yellow, 2 / Zoom);
            context.DrawLine(outer, Start, End);
            context.DrawLine(inner, Start, End);
            context.DrawEllipse(Brushes.Black, null, Start, 3 / Zoom, 3 / Zoom);
            context.DrawEllipse(Brushes.Yellow, null, Start, 1.5 / Zoom, 1.5 / Zoom);
        }
    }
}
