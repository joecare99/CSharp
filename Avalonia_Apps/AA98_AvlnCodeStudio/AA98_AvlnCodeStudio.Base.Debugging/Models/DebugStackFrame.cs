namespace AA98_AvlnCodeStudio.Base.Debugging.Models;

/// <summary>
/// Represents a source frame in a paused debug session.
/// </summary>
public sealed class DebugStackFrame
{
    /// <summary>
    /// Gets or sets the adapter-specific frame identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the method or function name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the source file path, when available.
    /// </summary>
    public string? SourcePath { get; set; }

    /// <summary>
    /// Gets or sets the one-based source line.
    /// </summary>
    public int Line { get; set; }

    /// <summary>
    /// Gets or sets the one-based source column.
    /// </summary>
    public int Column { get; set; }
}
