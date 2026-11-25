using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Avln_ImageEditor.Controls.Views;

/// <summary>Draws a non-interactive crop outline in image-pixel coordinates.</summary>
public sealed class CropPreviewOverlay : Control
{
    public static readonly StyledProperty<Rect> CropRectangleProperty =
        AvaloniaProperty.Register<CropPreviewOverlay, Rect>(nameof(CropRectangle));
    public static readonly StyledProperty<double> ZoomProperty =
        AvaloniaProperty.Register<CropPreviewOverlay, double>(nameof(Zoom), 1);

    static CropPreviewOverlay() => AffectsRender<CropPreviewOverlay>(CropRectangleProperty, ZoomProperty);

    public Rect CropRectangle
    {
        get => GetValue(CropRectangleProperty);
        set => SetValue(CropRectangleProperty, value);
    }

    public double Zoom
    {
        get => GetValue(ZoomProperty);
        set => SetValue(ZoomProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (CropRectangle.Width <= 0 || CropRectangle.Height <= 0 || !double.IsFinite(Zoom) || Zoom <= 0) return;
        context.DrawRectangle(null, new Pen(Brushes.Black, 3 / Zoom), CropRectangle);
        context.DrawRectangle(null, new Pen(Brushes.Yellow, 1 / Zoom), CropRectangle);
    }
}
