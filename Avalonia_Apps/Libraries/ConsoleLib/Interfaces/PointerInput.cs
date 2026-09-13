using System.Drawing;

namespace ConsoleLib.Interfaces;

/// <summary>
/// Immutable, platform-neutral pointer input payload.
/// </summary>
public readonly struct PointerInput
{
    public PointerInput(Point position, PointerInputKind kind, PointerButtons buttons = PointerButtons.None, int wheelDelta = 0, KeyModifiers modifiers = KeyModifiers.None)
    {
        Position = position;
        Kind = kind;
        Buttons = buttons;
        WheelDelta = wheelDelta;
        Modifiers = modifiers;
    }

    public Point Position { get; }
    public PointerInputKind Kind { get; }
    public PointerButtons Buttons { get; }
    public int WheelDelta { get; }
    public KeyModifiers Modifiers { get; }
}

public enum PointerInputKind
{
    Move,
    Press,
    Release,
    Wheel
}

[System.Flags]
public enum PointerButtons
{
    None = 0,
    Left = 1,
    Middle = 2,
    Right = 4
}
