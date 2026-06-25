using System.Collections.Generic;

namespace Terminal.Core;

/// <summary>
/// Represents an immutable snapshot of the current terminal viewport.
/// </summary>
public sealed class TerminalSnapshot
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TerminalSnapshot"/> class.
    /// </summary>
    /// <param name="size">The viewport size.</param>
    /// <param name="cursor">The cursor state.</param>
    /// <param name="lines">The visible viewport lines.</param>
    public TerminalSnapshot(TerminalSize size, TerminalCursorState cursor, IReadOnlyList<IReadOnlyList<TerminalCell>> lines)
    {
        Size = size;
        Cursor = cursor;
        Lines = lines;
    }

    /// <summary>
    /// Gets the viewport size.
    /// </summary>
    public TerminalSize Size { get; }

    /// <summary>
    /// Gets the cursor state.
    /// </summary>
    public TerminalCursorState Cursor { get; }

    /// <summary>
    /// Gets the visible viewport lines.
    /// </summary>
    public IReadOnlyList<IReadOnlyList<TerminalCell>> Lines { get; }
}
