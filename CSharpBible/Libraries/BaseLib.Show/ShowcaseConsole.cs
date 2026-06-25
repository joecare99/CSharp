using BaseLib.Interfaces;

namespace BaseLib.Show;

/// <summary>
/// Encapsulates the visual output conventions of the showcase application.
/// </summary>
internal sealed class ShowcaseConsole
{
    private readonly IConsole _console;
    private readonly ShowcaseText _text;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowcaseConsole"/> class.
    /// </summary>
    /// <param name="console">The console abstraction used for output.</param>
    /// <param name="text">The localized text provider.</param>
    public ShowcaseConsole(IConsole console, ShowcaseText text)
    {
        _console = console;
        _text = text;
    }

    /// <summary>
    /// Gets the underlying console abstraction.
    /// </summary>
    public IConsole RawConsole => _console;

    /// <summary>
    /// Writes an empty line.
    /// </summary>
    public void BlankLine() => _console.WriteLine();

    /// <summary>
    /// Clears the console.
    /// </summary>
    public void Clear() => _console.Clear();

    /// <summary>
    /// Prints a formatted section header.
    /// </summary>
    /// <param name="title">The section title.</param>
    public void PrintHeader(string title)
    {
        _console.ForegroundColor = ConsoleColor.Green;
        _console.WriteLine("══════════════════════════════════════════════════════════════════");
        _console.WriteLine($"  {title}");
        _console.WriteLine("══════════════════════════════════════════════════════════════════");
        _console.ForegroundColor = ConsoleColor.Gray;
        BlankLine();
    }

    /// <summary>
    /// Prints a formatted subsection header.
    /// </summary>
    /// <param name="title">The subsection title.</param>
    public void PrintSubHeader(string title)
    {
        _console.ForegroundColor = ConsoleColor.Cyan;
        _console.WriteLine($"  ── {title} ──");
        _console.ForegroundColor = ConsoleColor.Gray;
    }

    /// <summary>
    /// Prints a single labeled result line.
    /// </summary>
    /// <param name="description">The label shown before the value.</param>
    /// <param name="result">The value to display.</param>
    public void PrintResult(string description, object? result)
    {
        _console.ForegroundColor = ConsoleColor.White;
        _console.Write($"    {description}: ");
        _console.ForegroundColor = ConsoleColor.Yellow;
        _console.WriteLine($"{result}");
        _console.ForegroundColor = ConsoleColor.Gray;
    }

    /// <summary>
    /// Prints a muted code block before an example output.
    /// </summary>
    /// <param name="sourceCode">The source code to display.</param>
    public void PrintCode(string sourceCode)
    {
        _console.ForegroundColor = ConsoleColor.DarkGray;
        _console.WriteLine($"    {_text.Get("SourceCode_Label")}");
        foreach (string line in sourceCode.Replace("\r", string.Empty).Split('\n'))
        {
            _console.WriteLine($"      {line}");
        }
        _console.ForegroundColor = ConsoleColor.Gray;
        BlankLine();
    }

    /// <summary>
    /// Writes a standard line.
    /// </summary>
    /// <param name="text">The text to write.</param>
    public void WriteLine(string text = "") => _console.WriteLine(text);

    /// <summary>
    /// Writes text without a line terminator.
    /// </summary>
    /// <param name="text">The text to write.</param>
    public void Write(string text) => _console.Write(text);
}
