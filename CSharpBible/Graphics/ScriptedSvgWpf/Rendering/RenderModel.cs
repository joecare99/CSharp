using System.Collections.Generic;
using ScriptedSvgWpf.Models;

namespace ScriptedSvgWpf.Rendering;

public sealed class RenderDocument
{
    public RenderDocument(double width, double height, string background)
    {
        Width = width;
        Height = height;
        Background = background;
    }

    public double Width { get; }
    public double Height { get; }
    public string Background { get; set; }
    public List<RenderCommand> Commands { get; } = new();
}

public abstract record RenderCommand;

public sealed record RectangleCommand(
    double X,
    double Y,
    double Width,
    double Height,
    string Fill,
    double Scale = 1,
    double Rotation = 0) : RenderCommand
{
    public ScriptPoint Center => new(X + Width / 2, Y + Height / 2);
}

public sealed record CircleCommand(double CenterX, double CenterY, double Radius, string Fill) : RenderCommand;

public sealed record LineCommand(
    double X1,
    double Y1,
    double X2,
    double Y2,
    string Stroke,
    double StrokeWidth = 1) : RenderCommand;

public sealed record TextCommand(
    double X,
    double Y,
    string Text,
    string Fill,
    double FontSize = 14) : RenderCommand;

public sealed record PolygonCommand(IReadOnlyList<ScriptPoint> Points, string Fill, string? Stroke = null, double StrokeWidth = 1) : RenderCommand;

public sealed record PathCommand(string Data, string? Stroke = null, double StrokeWidth = 1, string? Fill = null) : RenderCommand;
