using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace VTileEdit.WPF.Helpers;

/// <summary>
/// Provides cached <see cref="SolidColorBrush"/> instances for each <see cref="ConsoleColor"/>.
/// </summary>
internal static class ConsoleColorBrushCache
{
    private static readonly Dictionary<ConsoleColor, SolidColorBrush> Cache = new();

    /// <summary>
    /// Gets a frozen brush representing the supplied console color.
    /// </summary>
    /// <param name="color">The console color to convert.</param>
    /// <returns>A cached <see cref="SolidColorBrush"/> instance.</returns>
    public static SolidColorBrush GetBrush(ConsoleColor color)
    {
        if (Cache.TryGetValue(color, out var brush))
        {
            return brush;
        }

        brush = CreateBrush(color);
        Cache[color] = brush;
        return brush;
    }

    private static SolidColorBrush CreateBrush(ConsoleColor color)
    {
        var mediaColor = color switch
        {
            ConsoleColor.Black => Color.FromRgb(0x00, 0x00, 0x00),
            ConsoleColor.DarkBlue => Color.FromRgb(0x00, 0x00, 0x80),
            ConsoleColor.DarkGreen => Color.FromRgb(0x00, 0x64, 0x00),
            ConsoleColor.DarkCyan => Color.FromRgb(0x00, 0x64, 0x64),
            ConsoleColor.DarkRed => Color.FromRgb(0x64, 0x00, 0x00),
            ConsoleColor.DarkMagenta => Color.FromRgb(0x64, 0x00, 0x64),
            ConsoleColor.DarkYellow => Color.FromRgb(0x80, 0x80, 0x00),
            ConsoleColor.Gray => Color.FromRgb(0xC0, 0xC0, 0xC0),
            ConsoleColor.DarkGray => Color.FromRgb(0x80, 0x80, 0x80),
            ConsoleColor.Blue => Color.FromRgb(0x00, 0x00, 0xFF),
            ConsoleColor.Green => Color.FromRgb(0x00, 0x80, 0x00),
            ConsoleColor.Cyan => Color.FromRgb(0x00, 0xFF, 0xFF),
            ConsoleColor.Red => Color.FromRgb(0xFF, 0x00, 0x00),
            ConsoleColor.Magenta => Color.FromRgb(0xFF, 0x00, 0xFF),
            ConsoleColor.Yellow => Color.FromRgb(0xFF, 0xFF, 0x00),
            ConsoleColor.White => Color.FromRgb(0xFF, 0xFF, 0xFF),
            _ => Color.FromRgb(0x80, 0x80, 0x80)
        };

        var brush = new SolidColorBrush(mediaColor);
        brush.Freeze();
        return brush;
    }
}
