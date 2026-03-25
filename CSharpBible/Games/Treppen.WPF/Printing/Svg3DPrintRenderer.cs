using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Treppen.Base;
using Treppen.WPF.Services.Drawing;
using Treppen.Export.Models;
using Treppen.Export.Services.Interfaces;
using Treppen.Print.Services;

namespace Treppen.WPF.Printing;

[PrintExporter("SVG", ".svg")]
public sealed class Svg3DPrintRenderer : IPrintRenderer
{
    public async Task RenderAsync(IHeightLabyrinth labyrinth, PrintOptions options, Stream output)
    {
        ArgumentNullException.ThrowIfNull(labyrinth);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(output);

        var dim = labyrinth.Dimension;
        int wCells = dim.Width;
        int hCells = dim.Height;
        if (wCells <= 0 || hCells <= 0)
            throw new InvalidOperationException("Leeres Labyrinth kann nicht gedruckt werden.");

        var drawer = BaseLib.Helper.IoC.GetRequiredService<ILabyrinth3dDrawer>();
        var factory = new SvgDrawCommandFactory();
        var drawWidth = Math.Max(1, options.CellSize) * wCells;
        var drawHeight = Math.Max(1, options.CellSize) * hCells;
        var commands = drawer.Build(labyrinth, new Size(drawWidth, drawHeight), factory);

        var allPoints = commands.SelectMany(c => c switch
        {
            SvgPolygonCommand p => p.Points,
            SvgPolyLineCommand l => l.Points,
            _ => Array.Empty<Point>()
        }).ToArray();

        double minX = allPoints.Length > 0 ? allPoints.Min(p => p.X) : 0;
        double minY = allPoints.Length > 0 ? allPoints.Min(p => p.Y) : 0;
        double maxX = allPoints.Length > 0 ? allPoints.Max(p => p.X) : drawWidth;
        double maxY = allPoints.Length > 0 ? allPoints.Max(p => p.Y) : drawHeight;

        double width = (maxX - minX) + options.MarginLeft + options.MarginRight;
        double height = (maxY - minY) + options.MarginTop + options.MarginBottom;

        var sb = new StringBuilder(128 * 1024);
        sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n");
        sb.Append($"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"{Fmt(width)}\" height=\"{Fmt(height)}\" viewBox=\"0 0 {Fmt(width)} {Fmt(height)}\">\n");
        sb.Append($"  <rect x=\"0\" y=\"0\" width=\"{Fmt(width)}\" height=\"{Fmt(height)}\" fill=\"{options.BackgroundColor}\" />\n");
        if (!string.IsNullOrWhiteSpace(options.Title))
        {
            sb.Append($"  <title>{Escape(options.Title!)}</title>\n");
        }

        sb.Append("  <g>\n");
        foreach (var cmd in commands)
        {
            switch (cmd)
            {
                case SvgPolygonCommand poly:
                    var pts = string.Join(" ", poly.Points.Select(p => $"{Fmt(options.MarginLeft + (p.X - minX))},{Fmt(options.MarginTop + (p.Y - minY))}"));
                    string fill = poly.Fill.HasValue ? ToSvgColor(poly.Fill.Value) : "none";
                    string stroke = poly.Stroke.HasValue ? ToSvgColor(poly.Stroke.Value) : "none";
                    string sw = poly.Stroke.HasValue ? Fmt(poly.StrokeThickness) : "0";
                    sb.Append($"    <polygon points=\"{pts}\" fill=\"{fill}\" stroke=\"{stroke}\" stroke-width=\"{sw}\" style=\"stroke-linejoin:bevel\" />\n");
                    break;
                case SvgPolyLineCommand line:
                    var lpts = string.Join(" ", line.Points.Select(p => $"{Fmt(options.MarginLeft + (p.X - minX))},{Fmt(options.MarginTop + (p.Y - minY))}"));
                    sb.Append($"    <polyline points=\"{lpts}\" fill=\"none\" stroke=\"{ToSvgColor(line.Stroke)}\" stroke-width=\"{Fmt(line.StrokeThickness)}\" {(line.DrawPoints?"style=\"stroke-linejoin:bevel;stroke-linecap:round\"":"")} />\n");
                    break;
            }
        }
        sb.Append("  </g>\n");
        sb.Append("</svg>");

        var utf8 = Encoding.UTF8.GetBytes(sb.ToString());
        await output.WriteAsync(utf8, 0, utf8.Length).ConfigureAwait(false);
    }

    private static string Escape(string text)
        => text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")
               .Replace("\"", "&quot;").Replace("'", "&apos;");

    private static string ToSvgColor(Color color)
        => $"rgb({color.R},{color.G},{color.B})";

    private static string Fmt(double d) => d.ToString("0.###", CultureInfo.InvariantCulture);
}
