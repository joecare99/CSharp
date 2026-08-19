using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using ScriptedSvgWpf.Rendering;

namespace ScriptedSvgWpf;

public sealed class PreviewSurface : FrameworkElement
{
    public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register(
        nameof(Document),
        typeof(RenderDocument),
        typeof(PreviewSurface),
        new FrameworkPropertyMetadata(
            null,
            FrameworkPropertyMetadataOptions.AffectsRender,
            static (dependencyObject, _) => ((PreviewSurface)dependencyObject).InvalidateVisual()));

    public RenderDocument? Document
    {
        get => (RenderDocument?)GetValue(DocumentProperty);
        set => SetValue(DocumentProperty, value);
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        InvalidateVisual();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        var document = Document;
        if (document is null || document.Width <= 0 || document.Height <= 0 || ActualWidth <= 0 || ActualHeight <= 0)
        {
            drawingContext.DrawRectangle(Brushes.WhiteSmoke, null, new Rect(0, 0, ActualWidth, ActualHeight));
            return;
        }

        var scale = Math.Min(ActualWidth / document.Width, ActualHeight / document.Height);
        var offsetX = (ActualWidth - document.Width * scale) / 2;
        var offsetY = (ActualHeight - document.Height * scale) / 2;
        drawingContext.DrawRectangle(Brushes.WhiteSmoke, null, new Rect(0, 0, ActualWidth, ActualHeight));
        drawingContext.PushTransform(new TranslateTransform(offsetX, offsetY));
        drawingContext.PushTransform(new ScaleTransform(scale, scale));
        drawingContext.PushClip(new RectangleGeometry(new Rect(0, 0, document.Width, document.Height)));
        drawingContext.DrawRectangle(ToBrush(document.Background), null, new Rect(0, 0, document.Width, document.Height));

        foreach (var command in document.Commands)
        {
            DrawCommand(drawingContext, command);
        }

        drawingContext.Pop();
        drawingContext.Pop();
        drawingContext.Pop();
    }

    private static void DrawCommand(DrawingContext context, RenderCommand command)
    {
        switch (command)
        {
            case RectangleCommand rectangle:
                var center = rectangle.Center;
                context.PushTransform(new TranslateTransform(center.X, center.Y));
                context.PushTransform(new RotateTransform(-rectangle.Rotation));
                context.PushTransform(new ScaleTransform(rectangle.Scale, rectangle.Scale));
                context.DrawRectangle(ToBrush(rectangle.Fill), null, new Rect(-rectangle.Width / 2, -rectangle.Height / 2, rectangle.Width, rectangle.Height));
                context.Pop();
                context.Pop();
                context.Pop();
                break;
            case CircleCommand circle:
                context.DrawEllipse(ToBrush(circle.Fill), null, new System.Windows.Point(circle.CenterX, circle.CenterY), circle.Radius, circle.Radius);
                break;
            case LineCommand line:
                context.DrawLine(new Pen(ToBrush(line.Stroke), line.StrokeWidth), new System.Windows.Point(line.X1, line.Y1), new System.Windows.Point(line.X2, line.Y2));
                break;
            case TextCommand text:
                var formatted = new FormattedText(
                    text.Text,
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Segoe UI"),
                    text.FontSize,
                    ToBrush(text.Fill),
                    1);
                context.DrawText(formatted, new System.Windows.Point(text.X, text.Y));
                break;
            case PolygonCommand polygon:
                var geometry = new StreamGeometry();
                using (var geometryContext = geometry.Open())
                {
                    if (polygon.Points.Count > 0)
                    {
                        geometryContext.BeginFigure(new System.Windows.Point(polygon.Points[0].X, polygon.Points[0].Y), true, true);
                        for (var index = 1; index < polygon.Points.Count; index++)
                        {
                            geometryContext.LineTo(new System.Windows.Point(polygon.Points[index].X, polygon.Points[index].Y), true, false);
                        }
                    }
                }

                context.DrawGeometry(
                    ToBrush(polygon.Fill),
                    polygon.Stroke is null ? null : new Pen(ToBrush(polygon.Stroke), polygon.StrokeWidth),
                    geometry);
                break;
            case PathCommand path:
                try
                {
                    context.DrawGeometry(
                        path.Fill is null ? null : ToBrush(path.Fill),
                        path.Stroke is null ? null : new Pen(ToBrush(path.Stroke), path.StrokeWidth),
                        Geometry.Parse(path.Data));
                }
                catch (FormatException)
                {
                }

                break;
        }
    }

    private static Brush ToBrush(string value)
    {
        try
        {
            var brush = new BrushConverter().ConvertFromString(value) as Brush;
            if (brush is not null)
            {
                brush.Freeze();
                return brush;
            }
        }
        catch (FormatException)
        {
        }

        return Brushes.Black;
    }
}
