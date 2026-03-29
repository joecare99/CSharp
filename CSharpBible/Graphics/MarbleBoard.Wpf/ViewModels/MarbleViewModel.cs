using MarbleBoard.Engine.Models;
using System.Windows.Media;

namespace MarbleBoard.Wpf.ViewModels;

/// <summary>
/// Represents a single marble prepared for WPF rendering.
/// </summary>
public sealed class MarbleViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MarbleViewModel"/> class.
    /// </summary>
    /// <param name="coordinate">The marble coordinate.</param>
    /// <param name="left">The left canvas position.</param>
    /// <param name="top">The top canvas position.</param>
    /// <param name="diameter">The marble diameter.</param>
    /// <param name="color">The marble color.</param>
    /// <param name="isSelected">A value indicating whether the marble is selected.</param>
    /// <param name="hasTopSlot">A value indicating whether a top board slot exists.</param>
    /// <param name="hasRightSlot">A value indicating whether a right board slot exists.</param>
    /// <param name="hasBottomSlot">A value indicating whether a bottom board slot exists.</param>
    /// <param name="hasLeftSlot">A value indicating whether a left board slot exists.</param>
    /// <param name="topNeighbor">The color of the top neighbor.</param>
    /// <param name="rightNeighbor">The color of the right neighbor.</param>
    /// <param name="bottomNeighbor">The color of the bottom neighbor.</param>
    /// <param name="leftNeighbor">The color of the left neighbor.</param>
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

    /// <summary>
    /// Gets the board coordinate of the marble.
    /// </summary>
    public BoardCoordinate Coordinate { get; }

    /// <summary>
    /// Gets the left canvas position.
    /// </summary>
    public double Left { get; }

    /// <summary>
    /// Gets the top canvas position.
    /// </summary>
    public double Top { get; }

    /// <summary>
    /// Gets the diameter of the marble.
    /// </summary>
    public double Diameter { get; }

    /// <summary>
    /// Gets the marble fill brush.
    /// </summary>
    public Brush FillBrush { get; }

    /// <summary>
    /// Gets the broad form shadow brush used to reinforce the spherical volume.
    /// </summary>
    public Brush FormShadowBrush { get; }

    /// <summary>
    /// Gets the brush for the marble rim.
    /// </summary>
    public Brush RimBrush { get; }

    /// <summary>
    /// Gets the top hole shadow brush.
    /// </summary>
    public Brush TopHoleShadowBrush { get; }

    /// <summary>
    /// Gets the right hole shadow brush.
    /// </summary>
    public Brush RightHoleShadowBrush { get; }

    /// <summary>
    /// Gets the bottom hole shadow brush.
    /// </summary>
    public Brush BottomHoleShadowBrush { get; }

    /// <summary>
    /// Gets the left hole shadow brush.
    /// </summary>
    public Brush LeftHoleShadowBrush { get; }

    /// <summary>
    /// Gets the opacity of the direct highlight.
    /// </summary>
    public double HighlightOpacity { get; }

    /// <summary>
    /// Gets the top reflection brush.
    /// </summary>
    public Brush TopReflectionBrush { get; }

    /// <summary>
    /// Gets the right reflection brush.
    /// </summary>
    public Brush RightReflectionBrush { get; }

    /// <summary>
    /// Gets the bottom reflection brush.
    /// </summary>
    public Brush BottomReflectionBrush { get; }

    /// <summary>
    /// Gets the left reflection brush.
    /// </summary>
    public Brush LeftReflectionBrush { get; }

    /// <summary>
    /// Gets a value indicating whether the marble is selected.
    /// </summary>
    public bool IsSelected { get; }

    private static Brush CreateFillBrush(MarbleColor color)
    {
        Color baseColor = GetBaseColor(color);
        RadialGradientBrush brush = new()
        {
            Center = new System.Windows.Point(0.38, 0.36),
            GradientOrigin = new System.Windows.Point(0.23, 0.20),
            RadiusX = 0.98,
            RadiusY = 0.98,
        };

        brush.GradientStops.Add(new GradientStop(Lighten(baseColor, 0.84), 0.00));
        brush.GradientStops.Add(new GradientStop(Lighten(baseColor, 0.58), 0.14));
        brush.GradientStops.Add(new GradientStop(Lighten(baseColor, 0.22), 0.34));
        brush.GradientStops.Add(new GradientStop(baseColor, 0.58));
        brush.GradientStops.Add(new GradientStop(Darken(baseColor, 0.26), 0.80));
        brush.GradientStops.Add(new GradientStop(Darken(baseColor, 0.58), 1.00));
        brush.Freeze();
        return brush;
    }

    private static Brush CreateFormShadowBrush()
    {
        LinearGradientBrush brush = new()
        {
            StartPoint = new System.Windows.Point(0.10, 0.08),
            EndPoint = new System.Windows.Point(0.96, 0.98),
        };

        brush.GradientStops.Add(new GradientStop(Color.FromArgb(0, 8, 6, 10), 0.00));
        brush.GradientStops.Add(new GradientStop(Color.FromArgb(0, 8, 6, 10), 0.40));
        brush.GradientStops.Add(new GradientStop(Color.FromArgb(34, 16, 12, 18), 0.68));
        brush.GradientStops.Add(new GradientStop(Color.FromArgb(72, 10, 8, 12), 1.00));
        brush.Freeze();
        return brush;
    }

    private static Brush CreateReflectionBrush(MarbleColor? color, ReflectionDirection direction)
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

        LinearGradientBrush brush = direction switch
        {
            ReflectionDirection.Top => CreateReflectionGradient(baseColor, alpha, new System.Windows.Point(1.0, 0.5), new System.Windows.Point(0.0, 0.7)),
            ReflectionDirection.Right => CreateReflectionGradient(baseColor, alpha, new System.Windows.Point(0.25, 0.0), new System.Windows.Point(0.90, 1.0)),
            ReflectionDirection.Bottom => CreateReflectionGradient(baseColor, alpha, new System.Windows.Point(0.0, 0.3), new System.Windows.Point(1.0, 0.9)),
            _ => CreateReflectionGradient(baseColor, alpha, new System.Windows.Point(0.75, 0.0), new System.Windows.Point(0.10, 1.0)),
        };

        brush.Freeze();
        return brush;
    }

    private static Brush CreateHoleShadowBrush(bool xHasSlot, bool xHasNeighbor, HoleShadowDirection direction)
    {
        if (!xHasSlot || xHasNeighbor)
        {
            return Brushes.Transparent;
        }

        LinearGradientBrush brush = direction switch
        {
            HoleShadowDirection.Top => CreateHoleShadowGradient(new System.Windows.Point(0.5, 0.0), new System.Windows.Point(0.5, 1.0), 52),
            HoleShadowDirection.Right => CreateHoleShadowGradient(new System.Windows.Point(1.0, 0.5), new System.Windows.Point(0.0, 0.5), 48),
            HoleShadowDirection.Bottom => CreateHoleShadowGradient(new System.Windows.Point(0.5, 1.0), new System.Windows.Point(0.5, 0.0), 38),
            _ => CreateHoleShadowGradient(new System.Windows.Point(0.0, 0.5), new System.Windows.Point(1.0, 0.5), 44),
        };

        brush.Freeze();
        return brush;
    }

    private static LinearGradientBrush CreateReflectionGradient(Color color, byte alpha, System.Windows.Point startPoint, System.Windows.Point endPoint)
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

    private static double GetHighlightOpacity(bool xTopNeighborExists, bool xLeftNeighborExists)
    {
        int iOccluderCount = (xTopNeighborExists ? 1 : 0) + (xLeftNeighborExists ? 1 : 0);
        return iOccluderCount switch
        {
            0 => 0.94,
            1 => 0.78,
            _ => 0.62,
        };
    }

    private static LinearGradientBrush CreateHoleShadowGradient(System.Windows.Point startPoint, System.Windows.Point endPoint, byte alpha)
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
