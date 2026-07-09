using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Terminal.Core;

namespace Terminal.Avalonia.Controls;

/// <summary>
/// Renders a terminal snapshot using a simple fixed-size cell grid.
/// </summary>
public sealed class TerminalViewportControl : Control
{
    /// <summary>
    /// Defines the snapshot property.
    /// </summary>
    public static readonly StyledProperty<TerminalSnapshot?> SnapshotProperty =
        AvaloniaProperty.Register<TerminalViewportControl, TerminalSnapshot?>(nameof(Snapshot));

    /// <summary>
    /// Defines the font size property.
    /// </summary>
    public static readonly StyledProperty<double> TerminalFontSizeProperty =
        AvaloniaProperty.Register<TerminalViewportControl, double>(nameof(TerminalFontSize), 14d);

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminalViewportControl"/> class.
    /// </summary>
    public TerminalViewportControl()
    {
        Focusable = true;
        ClipToBounds = true;
    }

    /// <summary>
    /// Gets or sets the terminal snapshot.
    /// </summary>
    public TerminalSnapshot? Snapshot
    {
        get => GetValue(SnapshotProperty);
        set => SetValue(SnapshotProperty, value);
    }

    /// <summary>
    /// Gets or sets the terminal font size.
    /// </summary>
    public double TerminalFontSize
    {
        get => GetValue(TerminalFontSizeProperty);
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
    public override void Render(DrawingContext context)
    {
        base.Render(context);
        var snapshot = Snapshot;
        if (snapshot is null)
        {
            return;
        }

        var cellWidth = GetCellWidth();
        var cellHeight = GetCellHeight();
        var typeface = new Typeface(new FontFamily("Cascadia Mono,Consolas,Menlo,Monospace"));

        for (var row = 0; row < snapshot.Lines.Count; row++)
        {
            var line = snapshot.Lines[row];
            for (var column = 0; column < line.Count; column++)
            {
                var cell = line[column];
                var rect = new Rect(column * cellWidth, row * cellHeight, cellWidth, cellHeight);
                context.FillRectangle(new SolidColorBrush(ToColor(cell.Background)), rect);

                var text = new FormattedText(
                    cell.Character.ToString(),
                    System.Globalization.CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    TerminalFontSize,
                    new SolidColorBrush(ToColor(cell.Foreground)));

                context.DrawText(text, new Point(rect.X, rect.Y));
            }
        }

        if (snapshot.Cursor.IsVisible)
        {
            var cursorRect = new Rect(snapshot.Cursor.Column * cellWidth, snapshot.Cursor.Row * cellHeight + cellHeight - 2, cellWidth, 2);
            context.FillRectangle(Brushes.White, cursorRect);
        }
    }

    private static Color ToColor(TerminalColor color)
    {
        return Color.FromRgb(color.Red, color.Green, color.Blue);
    }
}
