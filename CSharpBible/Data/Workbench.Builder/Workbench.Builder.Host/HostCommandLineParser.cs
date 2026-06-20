using System;
using Workbench.Builder.Cli;

namespace Workbench.Builder.Host;

/// <summary>
/// Parses command-line arguments for the builder compile host.
/// </summary>
public sealed class HostCommandLineParser : ProjectCommandLineParserBase<HostCommandOptions>
{
    /// <inheritdoc/>
    protected override bool TryHandleArgument(string[] args, ref int index, string argument, ref string? projectFilePath)
    {
        if (string.Equals(argument, HostArgumentNames.Output, StringComparison.OrdinalIgnoreCase))
        {
            if (index + 1 >= args.Length)
            {
                throw new ArgumentException("An output directory value is required after --output.", nameof(args));
            }

            OutputDirectory = args[++index];
            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    protected override HostCommandOptions CreateOptions(string? projectFilePath, bool showHelp)
    {
        HostCommandOptions options = new(projectFilePath, OutputDirectory, showHelp);
        OutputDirectory = null;
        return options;
    }

    private string? OutputDirectory { get; set; }
}
