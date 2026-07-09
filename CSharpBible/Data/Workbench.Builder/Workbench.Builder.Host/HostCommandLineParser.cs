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

        if (string.Equals(argument, HostArgumentNames.Verbosity, StringComparison.OrdinalIgnoreCase))
        {
            if (index + 1 >= args.Length)
            {
                throw new ArgumentException("A verbosity value is required after --verbosity. Use 'normal' or 'detailed'.", nameof(args));
            }

            Verbosity = ParseVerbosity(args[++index]);
            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    protected override HostCommandOptions CreateOptions(string? projectFilePath, bool showHelp)
    {
        HostCommandOptions options = new(projectFilePath, OutputDirectory, Verbosity, showHelp);
        OutputDirectory = null;
        Verbosity = HostVerbosity.Normal;
        return options;
    }

    private static HostVerbosity ParseVerbosity(string value)
    {
        if (string.Equals(value, "normal", StringComparison.OrdinalIgnoreCase))
        {
            return HostVerbosity.Normal;
        }

        if (string.Equals(value, "detailed", StringComparison.OrdinalIgnoreCase)
            || string.Equals(value, "verbose", StringComparison.OrdinalIgnoreCase))
        {
            return HostVerbosity.Detailed;
        }

        throw new ArgumentException($"The verbosity '{value}' is not supported. Use 'normal' or 'detailed'.", nameof(value));
    }

    private string? OutputDirectory { get; set; }

    private HostVerbosity Verbosity { get; set; }
}
