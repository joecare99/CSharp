using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Terminal.Core;

namespace Terminal.Wpf.Controls;

using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// Renders a terminal snapshot using a simple fixed-size cell grid.
/// </summary>
public sealed class TerminalViewportControl : FrameworkElement
{
    private static readonly Dictionary<int, SolidColorBrush> BrushCache = [];
    private static readonly Typeface TerminalTypeface = new(new FontFamily("Cascadia Mono, Consolas, Global Monospace"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

    /// <summary>
    /// Defines the snapshot property.
    /// </summary>
    public static readonly DependencyProperty SnapshotProperty = DependencyProperty.Register(
        nameof(Snapshot),
        typeof(TerminalSnapshot),
        typeof(TerminalViewportControl),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Defines the terminal font size property.
    /// </summary>
    public static readonly DependencyProperty TerminalFontSizeProperty = DependencyProperty.Register(
        nameof(TerminalFontSize),
        typeof(double),
        typeof(TerminalViewportControl),
        new FrameworkPropertyMetadata(14d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminalViewportControl"/> class.
    /// </summary>
    public TerminalViewportControl()
    {
        Focusable = true;
        SnapsToDevicePixels = true;
        UseLayoutRounding = true;
    }

    /// <summary>
    /// Gets or sets the terminal snapshot.
    /// </summary>
    public TerminalSnapshot? Snapshot
    {
        get => (TerminalSnapshot?)GetValue(SnapshotProperty);
        set => SetValue(SnapshotProperty, value);
    }

    /// <summary>
    /// Gets or sets the terminal font size.
    /// </summary>
    public double TerminalFontSize
    {
        get => (double)GetValue(TerminalFontSizeProperty);
        set => SetValue(TerminalFontSizeProperty, value);
    }

    /// <summary>
    /// Measures the approximate terminal cell width.
    /// </summary>
    public double GetCellWidth() => TerminalFontSize * 0.62d;

    /// <summary>
    /// Measures the approximate terminal cell height.
    /// </summary>
    public double GetCellHeight() => TerminalFontSize * 1.35d;

    /// <inheritdoc/>
    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        var snapshot = Snapshot;
        if (snapshot is null)
        {
            return;
        }

        var cellWidth = GetCellWidth();
        var cellHeight = GetCellHeight();
        var pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;

        for (var row = 0; row < snapshot.Lines.Count; row++)
        {
            var line = snapshot.Lines[row];
            for (var column = 0; column < line.Count; column++)
            {
                var cell = line[column];
                var rect = new Rect(column * cellWidth, row * cellHeight, cellWidth, cellHeight);
                drawingContext.DrawRectangle(GetBrush(cell.Background), null, rect);

                if (!TryGetRenderableText(cell.Character, out var textValue))
                {
                    continue;
                }

                var text = new FormattedText(
                    textValue,
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    TerminalTypeface,
                    TerminalFontSize,
                    GetBrush(cell.Foreground),
                    pixelsPerDip);

                drawingContext.DrawText(text, new Point(rect.X, rect.Y));
            }
        }

        if (snapshot.Cursor.IsVisible)
        {
            var cursorRect = new Rect(snapshot.Cursor.Column * cellWidth, snapshot.Cursor.Row * cellHeight + cellHeight - 2, cellWidth, 2);
            drawingContext.DrawRectangle(Brushes.White, null, cursorRect);
        }
    }

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size availableSize)
    {
        var snapshot = Snapshot;
        if (snapshot is null)
        {
            return new Size(GetCellWidth() * 80, GetCellHeight() * 25);
        }

        return new Size(snapshot.Size.Columns * GetCellWidth(), snapshot.Size.Rows * GetCellHeight());
    }

    private static Color ToColor(TerminalColor color)
    {
        return Color.FromRgb(color.Red, color.Green, color.Blue);
    }

    private static SolidColorBrush GetBrush(TerminalColor color)
    {
        var key = (color.Red << 16) | (color.Green << 8) | color.Blue;
        if (BrushCache.TryGetValue(key, out var brush))
        {
            return brush;
        }

        brush = new SolidColorBrush(ToColor(color));
        brush.Freeze();
        BrushCache[key] = brush;
        return brush;
    }

    private static bool TryGetRenderableText(char character, out string text)
    {
        text = string.Empty;

        if (character == ' ')
        {
            return false;
        }

        if (char.IsControl(character) || char.IsSurrogate(character))
        {
            Debug.WriteLine($"Skipping unsupported WPF render character U+{(int)character:X4}.");
            return false;
        }

        text = character.ToString();
        return true;
    }
}
