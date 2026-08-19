using System;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;

namespace ScriptedSvgWpf.Rendering;

public sealed class SvgExporter
{
    public string Export(RenderDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        var builder = new StringBuilder();
        builder.AppendLine($"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"{Number(document.Width)}\" height=\"{Number(document.Height)}\" viewBox=\"0 0 {Number(document.Width)} {Number(document.Height)}\">");
        builder.AppendLine($"  <rect width=\"100%\" height=\"100%\" fill=\"{Escape(document.Background)}\" />");

        foreach (var command in document.Commands)
        {
            switch (command)
            {
                case RectangleCommand rectangle:
                    AppendRectangle(builder, rectangle);
                    break;
                case CircleCommand circle:
                    builder.AppendLine($"  <circle cx=\"{Number(circle.CenterX)}\" cy=\"{Number(circle.CenterY)}\" r=\"{Number(circle.Radius)}\" fill=\"{Escape(circle.Fill)}\" />");
                    break;
                case LineCommand line:
                    builder.AppendLine($"  <line x1=\"{Number(line.X1)}\" y1=\"{Number(line.Y1)}\" x2=\"{Number(line.X2)}\" y2=\"{Number(line.Y2)}\" stroke=\"{Escape(line.Stroke)}\" stroke-width=\"{Number(line.StrokeWidth)}\" />");
                    break;
                case TextCommand text:
                    builder.AppendLine($"  <text x=\"{Number(text.X)}\" y=\"{Number(text.Y)}\" fill=\"{Escape(text.Fill)}\" font-size=\"{Number(text.FontSize)}\">{Escape(text.Text)}</text>");
                    break;
                case PolygonCommand polygon:
                    builder.AppendLine($"  <polygon points=\"{string.Join(" ", polygon.Points.Select(point => $"{Number(point.X)},{Number(point.Y)}"))}\" fill=\"{Escape(polygon.Fill)}\"{StrokeAttributes(polygon.Stroke, polygon.StrokeWidth)} />");
                    break;
                case PathCommand path:
                    builder.AppendLine($"  <path d=\"{Escape(path.Data)}\"{StrokeAttributes(path.Stroke, path.StrokeWidth)} fill=\"{Escape(path.Fill ?? "none")}\" />");
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported render command '{command.GetType().Name}'.");
            }
        }

        builder.AppendLine("</svg>");
        return builder.ToString();
    }

    private static void AppendRectangle(StringBuilder builder, RectangleCommand rectangle)
    {
        var center = rectangle.Center;
        var transform = $"translate({Number(center.X)} {Number(center.Y)}) rotate({Number(-rectangle.Rotation)}) scale({Number(rectangle.Scale)}) translate({Number(-center.X)} {Number(-center.Y)})";
        builder.AppendLine($"  <rect x=\"{Number(rectangle.X)}\" y=\"{Number(rectangle.Y)}\" width=\"{Number(rectangle.Width)}\" height=\"{Number(rectangle.Height)}\" fill=\"{Escape(rectangle.Fill)}\" transform=\"{transform}\" />");
    }

    private static string StrokeAttributes(string? stroke, double width)
    {
        return stroke is null
            ? string.Empty
            : $" stroke=\"{Escape(stroke)}\" stroke-width=\"{Number(width)}\"";
    }

    private static string Number(double value) => value.ToString("0.##########", CultureInfo.InvariantCulture);

    private static string Escape(string? value) => SecurityElement.Escape(value ?? string.Empty) ?? string.Empty;
}
