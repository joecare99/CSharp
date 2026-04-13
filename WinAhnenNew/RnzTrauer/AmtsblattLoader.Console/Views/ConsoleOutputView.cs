namespace AmtsblattLoader.Console.Views;

/// <summary>
/// Provides console-based output for the Amtsblatt console application.
/// </summary>
public sealed class ConsoleOutputView
{
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
