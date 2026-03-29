namespace MarbleBoard.Engine.Models;

/// <summary>
/// Represents a single board coordinate in the prototype grid.
/// </summary>
/// <param name="Column">The zero-based column index.</param>
/// <param name="Row">The zero-based row index.</param>
public readonly record struct BoardCoordinate(int Column, int Row)
{
    /// <summary>
    /// Creates a new coordinate by applying an offset to the current coordinate.
    /// </summary>
    /// <param name="columnOffset">The horizontal offset.</param>
    /// <param name="rowOffset">The vertical offset.</param>
    /// <returns>The offset coordinate.</returns>
    public BoardCoordinate Offset(int columnOffset, int rowOffset)
        => new(Column + columnOffset, Row + rowOffset);
}
