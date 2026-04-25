namespace FBParser;

/// <summary>
/// Provides a minimal parser base abstraction compatible with the original parser design.
/// </summary>
public abstract class ParserBase
{
    /// <summary>
    /// Gets the current one-based line number tracked by the parser.
    /// </summary>
    protected long LineNo { get; set; } = 1;

    /// <summary>
    /// Gets or sets the current one-based offset tracked by the parser.
    /// </summary>
    protected long Offset { get; set; }

    /// <summary>
    /// Gets or sets the buffered parser data.
    /// </summary>
    protected string FData { get; set; } = string.Empty;

    /// <summary>
    /// Feeds input text into the parser.
    /// </summary>
    /// <param name="text">The input text to parse.</param>
    public abstract void Feed(string text);

    /// <summary>
    /// Reports an error raised during parsing.
    /// </summary>
    /// <param name="sender">The sender that raised the error.</param>
    /// <param name="message">The error message.</param>
    public abstract void Error(object? sender, string message);

    /// <summary>
    /// Reports a warning raised during parsing.
    /// </summary>
    /// <param name="sender">The sender that raised the warning.</param>
    /// <param name="message">The warning message.</param>
    public abstract void Warning(object? sender, string message);

    /// <summary>
    /// Resets the base parser state.
    /// </summary>
    public virtual void Reset()
    {
        LineNo = 1;
        Offset = 0;
        FData = string.Empty;
    }

    /// <summary>
    /// Returns the current parser position.
    /// </summary>
    /// <returns>The tracked line number and one-based offset.</returns>
    public (long LineNo, long Offset) GetPos() => (LineNo, Offset);
}
