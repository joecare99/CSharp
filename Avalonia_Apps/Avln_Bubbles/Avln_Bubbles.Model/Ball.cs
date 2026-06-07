namespace Avln_Bubbles.Model;

/// <summary>
/// Represents a single bubble entry on the logical game board.
/// </summary>
public sealed class Ball
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Ball"/> class.
    /// </summary>
    /// <param name="x">The zero-based column.</param>
    /// <param name="y">The zero-based row.</param>
    /// <param name="type">The bubble type identifier.</param>
    public Ball(int x, int y, int type = 0)
    {
        X = x;
        Y = y;
        Type = type;
    }

    /// <summary>
    /// Gets or sets the zero-based column.
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// Gets or sets the zero-based row.
    /// </summary>
    public int Y { get; set; }

    /// <summary>
    /// Gets or sets the bubble type identifier.
    /// </summary>
    public int Type { get; set; }
}
