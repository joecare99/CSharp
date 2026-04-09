namespace RnzTrauer.Console.Views;

/// <summary>
/// Provides console-based output for the RNZ console application.
/// </summary>
public sealed class ConsoleOutputView
{
    /// <summary>
    /// Writes text without a trailing newline.
    /// </summary>
    public void Write(string sText)
    {
        System.Console.Write(sText);
    }

    /// <summary>
    /// Writes a single character without a trailing newline.
    /// </summary>
    public void Write(char cValue)
    {
        System.Console.Write(cValue);
    }

    /// <summary>
    /// Writes text followed by a newline.
    /// </summary>
    public void WriteLine(string sText = "")
    {
        System.Console.WriteLine(sText);
    }

    /// <summary>
    /// Writes an error line.
    /// </summary>
    public void WriteErrorLine(string sText)
    {
        System.Console.Error.WriteLine(sText);
    }
}
