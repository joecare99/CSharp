using Avalonia;
using Avalonia.Media;
using MarbleBoard.Engine.Models;

namespace Avln_Marble.Display.ViewModels;

/// <summary>
/// Represents a single marble prepared for Avalonia rendering.
/// </summary>
public sealed class MarbleViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MarbleViewModel"/> class.
    /// </summary>
    public MarbleViewModel(
        BoardCoordinate coordinate,
        double left,
        double top,
        double diameter,
        MarbleColor color,
        bool isSelected,
        bool hasTopSlot,
        bool hasRightSlot,
        bool hasBottomSlot,
        bool hasLeftSlot,
        MarbleColor? topNeighbor,
        MarbleColor? rightNeighbor,
        MarbleColor? bottomNeighbor,
        MarbleColor? leftNeighbor)
    {
        Coordinate = coordinate;
        Left = left;
        Top = top;
        Diameter = diameter;
        FillBrush = CreateFillBrush(color);
        FormShadowBrush = CreateFormShadowBrush();
        RimBrush = new SolidColorBrush(Darken(GetBaseColor(color), 0.38));
        TopHoleShadowBrush = CreateHoleShadowBrush(hasTopSlot, topNeighbor is not null, HoleShadowDirection.Top);
        RightHoleShadowBrush = CreateHoleShadowBrush(hasRightSlot, rightNeighbor is not null, HoleShadowDirection.Right);
        BottomHoleShadowBrush = CreateHoleShadowBrush(hasBottomSlot, bottomNeighbor is not null, HoleShadowDirection.Bottom);
        LeftHoleShadowBrush = CreateHoleShadowBrush(hasLeftSlot, leftNeighbor is not null, HoleShadowDirection.Left);
        TopReflectionBrush = CreateReflectionBrush(topNeighbor, ReflectionDirection.Top);
        RightReflectionBrush = CreateReflectionBrush(rightNeighbor, ReflectionDirection.Right);
        BottomReflectionBrush = CreateReflectionBrush(bottomNeighbor, ReflectionDirection.Bottom);
        LeftReflectionBrush = CreateReflectionBrush(leftNeighbor, ReflectionDirection.Left);
        HighlightOpacity = GetHighlightOpacity(topNeighbor is not null, leftNeighbor is not null);
        IsSelected = isSelected;
    }

    public BoardCoordinate Coordinate { get; }

    public double Left { get; }

    public double Top { get; }

    public double Diameter { get; }

    public IBrush FillBrush { get; }

    public IBrush FormShadowBrush { get; }

    public IBrush RimBrush { get; }

    public IBrush TopHoleShadowBrush { get; }

    public IBrush RightHoleShadowBrush { get; }

    public IBrush BottomHoleShadowBrush { get; }

    public IBrush LeftHoleShadowBrush { get; }

    public double HighlightOpacity { get; }

    public IBrush TopReflectionBrush { get; }

    public IBrush RightReflectionBrush { get; }

    public IBrush BottomReflectionBrush { get; }

    public IBrush LeftReflectionBrush { get; }

    public bool IsSelected { get; }

    private static IBrush CreateFillBrush(MarbleColor color)
    {
        Color baseColor = GetBaseColor(color);
        RadialGradientBrush brush = new()
        {
            Center = new RelativePoint(0.38, 0.36, RelativeUnit.Relative),
            GradientOrigin = new RelativePoint(0.23, 0.20, RelativeUnit.Relative),
            RadiusX = new RelativeScalar(0.98, RelativeUnit.Relative),
            RadiusY = new RelativeScalar(0.98, RelativeUnit.Relative),
        };

        brush.GradientStops.Add(new GradientStop(Lighten(baseColor, 0.84), 0.00));
        brush.GradientStops.Add(new GradientStop(Lighten(baseColor, 0.58), 0.14));
        brush.GradientStops.Add(new GradientStop(Lighten(baseColor, 0.22), 0.34));
        brush.GradientStops.Add(new GradientStop(baseColor, 0.58));
        brush.GradientStops.Add(new GradientStop(Darken(baseColor, 0.26), 0.80));
        brush.GradientStops.Add(new GradientStop(Darken(baseColor, 0.58), 1.00));
        return brush;
    }

    private static IBrush CreateFormShadowBrush()
    {
        LinearGradientBrush brush = new()
        {
            StartPoint = new RelativePoint(0.10, 0.08, RelativeUnit.Relative),
            EndPoint = new RelativePoint(0.96, 0.98, RelativeUnit.Relative),
        };

        brush.GradientStops.Add(new GradientStop(Color.FromArgb(0, 8, 6, 10), 0.00));
        brush.GradientStops.Add(new GradientStop(Color.FromArgb(0, 8, 6, 10), 0.40));
        brush.GradientStops.Add(new GradientStop(Color.FromArgb(34, 16, 12, 18), 0.68));
        brush.GradientStops.Add(new GradientStop(Color.FromArgb(72, 10, 8, 12), 1.00));
        return brush;
    }

    private static IBrush CreateReflectionBrush(MarbleColor? color, ReflectionDirection direction)
    {
        if (color is null)
        {
            return Brushes.Transparent;
        }

        Color baseColor = GetBaseColor(color.Value);
        byte alpha = direction switch
        {
            ReflectionDirection.Top => 74,
            ReflectionDirection.Left => 64,
            ReflectionDirection.Right => 50,
            _ => 42,
        };

        return direction switch
        {
            ReflectionDirection.Top => CreateReflectionGradient(baseColor, alpha, new RelativePoint(1.0, 0.5, RelativeUnit.Relative), new RelativePoint(0.0, 0.7, RelativeUnit.Relative)),
            ReflectionDirection.Right => CreateReflectionGradient(baseColor, alpha, new RelativePoint(0.25, 0.0, RelativeUnit.Relative), new RelativePoint(0.90, 1.0, RelativeUnit.Relative)),
            ReflectionDirection.Bottom => CreateReflectionGradient(baseColor, alpha, new RelativePoint(0.0, 0.3, RelativeUnit.Relative), new RelativePoint(1.0, 0.9, RelativeUnit.Relative)),
            _ => CreateReflectionGradient(baseColor, alpha, new RelativePoint(0.75, 0.0, RelativeUnit.Relative), new RelativePoint(0.10, 1.0, RelativeUnit.Relative)),
        };
    }

    private static IBrush CreateHoleShadowBrush(bool hasSlot, bool hasNeighbor, HoleShadowDirection direction)
    {
        if (!hasSlot || hasNeighbor)
        {
            return Brushes.Transparent;
        }

        return direction switch
        {
            HoleShadowDirection.Top => CreateHoleShadowGradient(new RelativePoint(0.5, 0.0, RelativeUnit.Relative), new RelativePoint(0.5, 1.0, RelativeUnit.Relative), 52),
            HoleShadowDirection.Right => CreateHoleShadowGradient(new RelativePoint(1.0, 0.5, RelativeUnit.Relative), new RelativePoint(0.0, 0.5, RelativeUnit.Relative), 48),
            HoleShadowDirection.Bottom => CreateHoleShadowGradient(new RelativePoint(0.5, 1.0, RelativeUnit.Relative), new RelativePoint(0.5, 0.0, RelativeUnit.Relative), 38),
            _ => CreateHoleShadowGradient(new RelativePoint(0.0, 0.5, RelativeUnit.Relative), new RelativePoint(1.0, 0.5, RelativeUnit.Relative), 44),
        };
    }

    private static LinearGradientBrush CreateReflectionGradient(Color color, byte alpha, RelativePoint startPoint, RelativePoint endPoint)
    {
        LinearGradientBrush brush = new()
        {
            StartPoint = startPoint,
            EndPoint = endPoint,
        };

        brush.GradientStops.Add(new GradientStop(Color.FromArgb(alpha, color.R, color.G, color.B), 0.00));
        brush.GradientStops.Add(new GradientStop(Color.FromArgb((byte)(alpha * 0.72), color.R, color.G, color.B), 0.24));
        brush.GradientStops.Add(new GradientStop(Color.FromArgb((byte)(alpha * 0.42), color.R, color.G, color.B), 0.52));
        brush.GradientStops.Add(new GradientStop(Color.FromArgb((byte)(alpha * 0.16), color.R, color.G, color.B), 0.78));
        brush.GradientStops.Add(new GradientStop(Color.FromArgb(0, color.R, color.G, color.B), 1.00));
        return brush;
    }

    private static double GetHighlightOpacity(bool topNeighborExists, bool leftNeighborExists)
    {
        int occluderCount = (topNeighborExists ? 1 : 0) + (leftNeighborExists ? 1 : 0);
        return occluderCount switch
        {
            0 => 0.94,
            1 => 0.78,
            _ => 0.62,
        };
    }

    private static LinearGradientBrush CreateHoleShadowGradient(RelativePoint startPoint, RelativePoint endPoint, byte alpha)
    {
        LinearGradientBrush brush = new()
        {
            StartPoint = startPoint,
            EndPoint = endPoint,
        };

        brush.GradientStops.Add(new GradientStop(Color.FromArgb(alpha, 18, 12, 8), 0.00));
        brush.GradientStops.Add(new GradientStop(Color.FromArgb((byte)(alpha * 0.55), 18, 12, 8), 0.28));
        brush.GradientStops.Add(new GradientStop(Color.FromArgb((byte)(alpha * 0.12), 18, 12, 8), 0.62));
        brush.GradientStops.Add(new GradientStop(Color.FromArgb(0, 18, 12, 8), 1.00));
        return brush;
    }

    private static Color GetBaseColor(MarbleColor color)
        => color switch
        {
            MarbleColor.Sapphire => Color.FromRgb(45, 116, 224),
            MarbleColor.Ruby => Color.FromRgb(196, 48, 70),
            MarbleColor.Emerald => Color.FromRgb(38, 162, 98),
            MarbleColor.Amber => Color.FromRgb(219, 166, 47),
            MarbleColor.Violet => Color.FromRgb(128, 88, 202),
            _ => Color.FromRgb(230, 232, 240),
        };

    private static Color Lighten(Color color, double factor)
        => Color.FromRgb(
            (byte)(color.R + ((255 - color.R) * factor)),
            (byte)(color.G + ((255 - color.G) * factor)),
            (byte)(color.B + ((255 - color.B) * factor)));

    private static Color Darken(Color color, double factor)
        => Color.FromRgb(
            (byte)(color.R * (1.0 - factor)),
            (byte)(color.G * (1.0 - factor)),
            (byte)(color.B * (1.0 - factor)));

    private enum HoleShadowDirection
    {
        Top,
        Right,
        Bottom,
        Left,
    }

    private enum ReflectionDirection
    {
        Top,
        Right,
        Bottom,
        Left,
    }
}
