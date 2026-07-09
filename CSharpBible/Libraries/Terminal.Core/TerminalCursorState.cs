namespace Terminal.Core;

/// <summary>
/// Represents the cursor state of the terminal viewport.
/// </summary>
public readonly record struct TerminalCursorState(int Column, int Row, bool IsVisible);
