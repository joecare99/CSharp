using System;
using System.IO;

namespace Avln_TerminalHost.Services;

/// <summary>
/// Resolves the Windows command interpreter path from the environment.
/// </summary>
public sealed class ComSpecLocator : IComSpecLocator
{
    /// <inheritdoc/>
    public string GetShellPath()
    {
        var comSpec = Environment.GetEnvironmentVariable("ComSpec");
        if (!string.IsNullOrWhiteSpace(comSpec) && File.Exists(comSpec))
        {
            return comSpec;
        }

        return "cmd.exe";
    }
}
