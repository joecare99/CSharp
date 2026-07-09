namespace Terminal.Core;

/// <summary>
/// Represents the terminal viewport size in character cells.
/// </summary>
public readonly record struct TerminalSize(int Columns, int Rows)
{
    /// <summary>
    /// Returns a size with a minimum extent of one cell in each dimension.
    /// </summary>
    public TerminalSize Normalize() => new(Columns < 1 ? 1 : Columns, Rows < 1 ? 1 : Rows);
}
