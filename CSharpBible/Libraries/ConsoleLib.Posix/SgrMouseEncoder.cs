using System;
using System.Drawing;
using ConsoleLib.Interfaces;

namespace ConsoleLib.Posix;

/// <summary>Encodes common pointer events as SGR (1006) mouse reports.</summary>
public static class SgrMouseEncoder
{
    public static string EnableTracking => "\u001b[?1000h\u001b[?1006h";
    public static string DisableTracking => "\u001b[?1006l\u001b[?1000l";

    public static string Encode(PointerInput input)
    {
        var button = input.Kind == PointerInputKind.Wheel
            ? (input.WheelDelta >= 0 ? 64 : 65)
            : input.Buttons switch
            {
                PointerButtons.Middle => 1,
                PointerButtons.Right => 2,
                _ => 0
            };
        if (input.Kind == PointerInputKind.Move) button |= 32;
        if (input.Kind == PointerInputKind.Move && input.Buttons == PointerButtons.None) button = 35;
        if ((input.Modifiers & KeyModifiers.Shift) != 0) button |= 4;
        if ((input.Modifiers & KeyModifiers.Alt) != 0) button |= 8;
        if ((input.Modifiers & KeyModifiers.Control) != 0) button |= 16;
        var column = Math.Max(1, input.Position.X + 1);
        var row = Math.Max(1, input.Position.Y + 1);
        var suffix = input.Kind == PointerInputKind.Release ? 'm' : 'M';
        return $"\u001b[<{button};{column};{row}{suffix}";
    }
}
