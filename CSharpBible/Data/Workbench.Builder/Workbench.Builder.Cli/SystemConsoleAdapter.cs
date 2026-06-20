using System;

namespace Workbench.Builder.Cli;

/// <summary>
/// Adapts <see cref="Console"/> to <see cref="IHostConsole"/>.
/// </summary>
public sealed class SystemConsoleAdapter : IHostConsole
{
    /// <inheritdoc/>
    public void WriteLine(string text)
    {
        Console.Out.WriteLine(text);
    }

    /// <inheritdoc/>
    public void WriteErrorLine(string text)
    {
        Console.Error.WriteLine(text);
    }
}
