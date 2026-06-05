using System;
using System.Globalization;
using Avln_TestConsole.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;

namespace Avln_TestConsole.Controls;

/// <summary>
/// Renders a console buffer using Avalonia drawing primitives and forwards keyboard input into the buffer.
/// </summary>
public class AvaloniaConsoleControl : Control
{
    private static readonly Typeface ConsoleTypeface = new("Consolas");
    private ConsoleBuffer? _buffer;

    /// <summary>
    /// Gets or sets the font size used to render the console content.
    /// </summary>
    public double CellFontSize { get; set; } = 14d;

    /// <summary>
    /// Gets or sets the backing console buffer.
    /// </summary>
    public ConsoleBuffer? Buffer
    {
        get => _buffer;
        set
        {
            if (ReferenceEquals(_buffer, value))
            {
                return;
            }

            if (_buffer is not null)
            {
                _buffer.BufferChanged -= OnBufferChanged;
            }

            _buffer = value;
            if (_buffer is not null)
            {
                _buffer.BufferChanged += OnBufferChanged;
            }

            InvalidateMeasure();
            InvalidateVisual();
        }
    }

    /// <summary>
    /// Gets the current rendered cell size for a single console character.
    /// </summary>
    public Size CharacterCellSize => CalculateCellSize();

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size availableSize)
    {
        if (_buffer is null)
        {
            return base.MeasureOverride(availableSize);
        }

        var cellSize = CalculateCellSize();
        return new Size(_buffer.WindowWidth * cellSize.Width, _buffer.WindowHeight * cellSize.Height);
    }

    /// <inheritdoc/>
    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (_buffer is null)
        {
            return;
        }

        var cellSize = CalculateCellSize();
        var cells = _buffer.ScreenBuffer;
        for (var index = 0; index < cells.Count; index++)
        {
            var cell = cells[index];
            var x = index % _buffer.WindowWidth;
            var y = index / _buffer.WindowWidth;
            var cellBounds = new Rect(x * cellSize.Width, y * cellSize.Height, cellSize.Width, cellSize.Height);
            context.FillRectangle(new SolidColorBrush(MapConsoleColor(cell.BackgroundColor)), cellBounds);

            if (cell.Character == '\0')
            {
                continue;
            }

            var text = new FormattedText(
                cell.Character.ToString(),
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                ConsoleTypeface,
                CellFontSize,
                new SolidColorBrush(MapConsoleColor(cell.ForegroundColor)));
            context.DrawText(text, new Point(cellBounds.X, cellBounds.Y));
        }

        if (IsFocused)
        {
            var cursor = _buffer.CursorPosition;
            var cursorBounds = new Rect(cursor.Left * cellSize.Width, cursor.Top * cellSize.Height, Math.Max(1d, cellSize.Width / 6d), cellSize.Height);
            context.FillRectangle(Brushes.White, cursorBounds);
        }
    }

    /// <inheritdoc/>
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        Focus();
    }

    /// <inheritdoc/>
    protected override void OnTextInput(TextInputEventArgs e)
    {
        base.OnTextInput(e);
        if (_buffer is null || string.IsNullOrEmpty(e.Text))
        {
            return;
        }

        foreach (var character in e.Text)
        {
            _buffer.EnqueueKey(new ConsoleKeyInfo(character, ConsoleKey.NoName, false, false, false));
        }
    }

    /// <inheritdoc/>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (_buffer is null)
        {
            return;
        }

        if (TryMapSpecialKey(e.Key, out var keyInfo))
        {
            _buffer.EnqueueKey(keyInfo);
            e.Handled = true;
        }
    }

    private static Color MapConsoleColor(ConsoleColor consoleColor)
        => consoleColor switch
        {
            ConsoleColor.Black => Color.FromRgb(0, 0, 0),
            ConsoleColor.DarkBlue => Color.FromRgb(0, 0, 128),
            ConsoleColor.DarkGreen => Color.FromRgb(0, 128, 0),
            ConsoleColor.DarkCyan => Color.FromRgb(0, 128, 128),
            ConsoleColor.DarkRed => Color.FromRgb(128, 0, 0),
            ConsoleColor.DarkMagenta => Color.FromRgb(128, 0, 128),
            ConsoleColor.DarkYellow => Color.FromRgb(128, 128, 0),
            ConsoleColor.Gray => Color.FromRgb(192, 192, 192),
            ConsoleColor.DarkGray => Color.FromRgb(64, 64, 64),
            ConsoleColor.Blue => Color.FromRgb(0, 0, 255),
            ConsoleColor.Green => Color.FromRgb(0, 255, 0),
            ConsoleColor.Cyan => Color.FromRgb(0, 255, 255),
            ConsoleColor.Red => Color.FromRgb(255, 0, 0),
            ConsoleColor.Magenta => Color.FromRgb(255, 0, 255),
            ConsoleColor.Yellow => Color.FromRgb(255, 255, 0),
            ConsoleColor.White => Color.FromRgb(255, 255, 255),
            _ => Color.FromRgb(192, 192, 192),
        };

    private void OnBufferChanged(object? sender, EventArgs e)
        => Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            InvalidateMeasure();
            InvalidateVisual();
        });

    private Size CalculateCellSize()
    {
        var sampleText = new FormattedText(
            "W",
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            ConsoleTypeface,
            CellFontSize,
            Brushes.White);
        return new Size(Math.Max(1d, sampleText.WidthIncludingTrailingWhitespace), Math.Max(1d, sampleText.Height));
    }

    private static bool TryMapSpecialKey(Key key, out ConsoleKeyInfo keyInfo)
    {
        switch (key)
        {
            case Key.Enter:
                keyInfo = new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false);
                return true;
            case Key.Back:
                keyInfo = new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false);
                return true;
            case Key.Tab:
                keyInfo = new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false);
                return true;
            default:
                keyInfo = default;
                return false;
        }
    }
}
