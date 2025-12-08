using System;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TShape component.
/// </summary>
public partial class TShape : LfmComponentBase
{
    [ObservableProperty]
    private EShapeType _shapeKind = EShapeType.Rectangle;

    [ObservableProperty]
    private Color _brushColor = Colors.White;

    [ObservableProperty]
    private EBrushStyle _brushStyleKind = EBrushStyle.Solid;

    [ObservableProperty]
    private Color _penColor = Colors.Black;

    [ObservableProperty]
    private int _penWidth = 1;

    [ObservableProperty]
    private EPenStyle _penStyleKind = EPenStyle.Solid;

    public TShape()
    {
        Height = 65;
        Width = 65;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "shape":
                ShapeKind = ParseShape(value?.ToString());
                break;
            case "brush.color":
                BrushColor = ConvertToColor(value, Colors.White);
                break;
            case "brush.style":
                BrushStyleKind = ParseBrushStyle(value?.ToString());
                break;
            case "pen.color":
                PenColor = ConvertToColor(value, Colors.Black);
                break;
            case "pen.width":
                PenWidth = ConvertToInt(value, 1);
                break;
            case "pen.style":
                PenStyleKind = ParsePenStyle(value?.ToString());
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }

    private static EShapeType ParseShape(string? value) => value?.ToLower() switch
    {
        "strectangle" => EShapeType.Rectangle,
        "stsquare" => EShapeType.Square,
        "stroundrect" => EShapeType.RoundRect,
        "stroundsquare" => EShapeType.RoundSquare,
        "stellipse" => EShapeType.Ellipse,
        "stcircle" => EShapeType.Circle,
        "stdiamond" => EShapeType.Diamond,
        "sttriangle" => EShapeType.Triangle,
        "sttriangleright" => EShapeType.TriangleRight,
        "sttriangleleft" => EShapeType.TriangleLeft,
        "sttriangledown" => EShapeType.TriangleDown,
        "ststar" => EShapeType.Star,
        _ => EShapeType.Rectangle
    };

    private static EBrushStyle ParseBrushStyle(string? value) => value?.ToLower() switch
    {
        "bssolid" => EBrushStyle.Solid,
        "bsclear" => EBrushStyle.Clear,
        "bshorizontal" => EBrushStyle.Horizontal,
        "bsvertical" => EBrushStyle.Vertical,
        "bsfdiagonal" => EBrushStyle.FDiagonal,
        "bsbdiagonal" => EBrushStyle.BDiagonal,
        "bscross" => EBrushStyle.Cross,
        "bsdiagcross" => EBrushStyle.DiagCross,
        _ => EBrushStyle.Solid
    };

    private static EPenStyle ParsePenStyle(string? value) => value?.ToLower() switch
    {
        "pssolid" => EPenStyle.Solid,
        "psdash" => EPenStyle.Dash,
        "psdot" => EPenStyle.Dot,
        "psdashdot" => EPenStyle.DashDot,
        "psdashdotdot" => EPenStyle.DashDotDot,
        "psclear" => EPenStyle.Clear,
        "psinsideframe" => EPenStyle.InsideFrame,
        _ => EPenStyle.Solid
    };
}
