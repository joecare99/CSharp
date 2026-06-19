namespace Workbench.Builder.Host;

/// <summary>
/// Abstracts console output for the thin builder host.
/// </summary>
public interface IHostConsole
{
    /// <summary>
    /// Writes a line to standard output.
    /// </summary>
    /// <param name="text">The text to write.</param>
    void WriteLine(string text);

    /// <summary>
    /// Writes a line to standard error.
    /// </summary>
    /// <param name="text">The text to write.</param>
    void WriteErrorLine(string text);
}
