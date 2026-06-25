using System;

namespace Terminal.Core;

/// <summary>
/// Provides the default ANSI color palette used by the terminal buffer.
/// </summary>
public static class TerminalPalette
{
    private static readonly TerminalColor[] StandardColors =
    [
        new(12, 12, 12),
        new(197, 15, 31),
        new(19, 161, 14),
        new(193, 156, 0),
        new(0, 55, 218),
        new(136, 23, 152),
        new(58, 150, 221),
        new(204, 204, 204),
        new(118, 118, 118),
        new(231, 72, 86),
        new(22, 198, 12),
        new(249, 241, 165),
        new(59, 120, 255),
        new(180, 0, 158),
        new(97, 214, 214),
        new(242, 242, 242)
    ];

    /// <summary>
    /// Resolves an ANSI SGR color code to a terminal color.
    /// </summary>
    public static TerminalColor GetColor(int code)
    {
        return code switch
        {
            >= 30 and <= 37 => StandardColors[code - 30],
            >= 40 and <= 47 => StandardColors[code - 40],
            >= 90 and <= 97 => StandardColors[8 + code - 90],
            >= 100 and <= 107 => StandardColors[8 + code - 100],
            _ => throw new ArgumentOutOfRangeException(nameof(code))
        };
    }

    /// <summary>
    /// Resolves an ANSI 256-color palette index to a terminal color.
    /// </summary>
    public static TerminalColor GetExtendedColor(int index)
    {
        return index switch
        {
            >= 0 and <= 15 => StandardColors[index],
            >= 16 and <= 231 => GetColorCubeColor(index - 16),
            >= 232 and <= 255 => GetGrayscaleColor(index - 232),
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };
    }

    private static TerminalColor GetColorCubeColor(int index)
    {
        var red = index / 36;
        var green = (index % 36) / 6;
        var blue = index % 6;
        return new TerminalColor(GetCubeComponent(red), GetCubeComponent(green), GetCubeComponent(blue));
    }

    private static TerminalColor GetGrayscaleColor(int index)
    {
        var component = (byte)(8 + (index * 10));
        return new TerminalColor(component, component, component);
    }

    private static byte GetCubeComponent(int value)
    {
        return value == 0 ? (byte)0 : (byte)(55 + (value * 40));
    }
}
