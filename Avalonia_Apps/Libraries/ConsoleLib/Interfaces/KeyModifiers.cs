namespace ConsoleLib.Interfaces;

/// <summary>
/// Describes modifier keys associated with a logical input event.
/// </summary>
[System.Flags]
public enum KeyModifiers
{
    None = 0,
    Shift = 1,
    Alt = 2,
    Control = 4,
    Meta = 8
}
