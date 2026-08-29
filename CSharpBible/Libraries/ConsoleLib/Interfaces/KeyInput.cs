using System;

namespace ConsoleLib.Interfaces;

/// <summary>
/// Immutable, platform-neutral keyboard input payload.
/// </summary>
public readonly struct KeyInput
{
    public KeyInput(ConsoleKey key, char keyChar, KeyModifiers modifiers, bool isKeyDown, bool isRepeat = false)
    {
        Key = key;
        KeyChar = keyChar;
        Modifiers = modifiers;
        IsKeyDown = isKeyDown;
        IsRepeat = isRepeat;
    }

    public ConsoleKey Key { get; }
    public char KeyChar { get; }
    public KeyModifiers Modifiers { get; }
    public bool IsKeyDown { get; }
    public bool IsRepeat { get; }
}
