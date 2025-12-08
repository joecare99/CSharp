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

public enum EShapeType
{
    Rectangle,
    Square,
    RoundRect,
    RoundSquare,
    Ellipse,
    Circle,
    Diamond,
    Triangle,
    TriangleRight,
    TriangleLeft,
    TriangleDown,
    Star
}

public enum EBrushStyle
{
    Solid,
    Clear,
    Horizontal,
    Vertical,
    FDiagonal,
    BDiagonal,
    Cross,
    DiagCross
}

public enum EPenStyle
{
    Solid,
    Dash,
    Dot,
    DashDot,
    DashDotDot,
    Clear,
    InsideFrame
}

/// <summary>
/// Represents a TBevel component.
/// </summary>
public partial class TBevel : LfmComponentBase
{
    [ObservableProperty]
    private EBevelShape _bevelShapeKind = EBevelShape.Box;

    [ObservableProperty]
    private EBevelStyle _bevelStyleKind = EBevelStyle.Lowered;

    public TBevel()
    {
        Height = 50;
        Width = 50;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "shape":
                BevelShapeKind = ParseShape(value?.ToString());
                break;
            case "style":
                BevelStyleKind = ParseStyle(value?.ToString());
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }

    private static EBevelShape ParseShape(string? value) => value?.ToLower() switch
    {
        "bsbox" => EBevelShape.Box,
        "bsframe" => EBevelShape.Frame,
        "bstopline" => EBevelShape.TopLine,
        "bsbottomline" => EBevelShape.BottomLine,
        "bsleftline" => EBevelShape.LeftLine,
        "bsrightline" => EBevelShape.RightLine,
        "bsspacer" => EBevelShape.Spacer,
        _ => EBevelShape.Box
    };

    private static EBevelStyle ParseStyle(string? value) => value?.ToLower() switch
    {
        "bslowered" => EBevelStyle.Lowered,
        "bsraised" => EBevelStyle.Raised,
        _ => EBevelStyle.Lowered
    };
}

public enum EBevelShape
{
    Box,
    Frame,
    TopLine,
    BottomLine,
    LeftLine,
    RightLine,
    Spacer
}

public enum EBevelStyle
{
    Lowered,
    Raised
}

/// <summary>
/// Represents a TSplitter component.
/// </summary>
public partial class TSplitter : LfmComponentBase
{
    [ObservableProperty]
    private int _minSize = 30;

    [ObservableProperty]
    private bool _beveled = true;

    public TSplitter()
    {
        Width = 5;
        Height = 100;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "minsize":
                MinSize = ConvertToInt(value, 30);
                break;
            case "beveled":
                Beveled = ConvertToBool(value, true);
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }
}
