namespace SharpHack.Persist.Models;

/// <summary>
/// Represents a serializable map or world coordinate.
/// </summary>
public sealed class SavePointDto
{
    /// <summary>
    /// Gets or sets the X coordinate.
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// Gets or sets the Y coordinate.
    /// </summary>
    public int Y { get; set; }
}
