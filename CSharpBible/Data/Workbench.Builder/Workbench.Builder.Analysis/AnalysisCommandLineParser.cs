using System;
using Workbench.Builder.Cli;
using Workbench.Builder.Core.Models.Inspection;

namespace Workbench.Builder.Analysis;

/// <summary>
/// Parses command-line arguments for the builder analysis host.
/// </summary>
public sealed class AnalysisCommandLineParser : ProjectCommandLineParserBase<AnalysisCommandOptions>
{
    /// <inheritdoc/>
    protected override bool TryHandleArgument(string[] args, ref int index, string argument, ref string? projectFilePath)
    {
        if (string.Equals(argument, AnalysisArgumentNames.Format, StringComparison.OrdinalIgnoreCase))
        {
            if (index + 1 >= args.Length)
            {
                throw new ArgumentException("A format value is required after --format.", nameof(args));
            }

            OutputFormat = ParseOutputFormat(args[++index]);
            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    protected override AnalysisCommandOptions CreateOptions(string? projectFilePath, bool showHelp)
    {
        AnalysisCommandOptions options = new(projectFilePath, OutputFormat, showHelp);
        OutputFormat = ProjectInspectionOutputFormat.PlainText;
        return options;
    }

    private static ProjectInspectionOutputFormat ParseOutputFormat(string value)
    {
        if (string.Equals(value, "plain", StringComparison.OrdinalIgnoreCase)
            || string.Equals(value, "text", StringComparison.OrdinalIgnoreCase)
            || string.Equals(value, "plaintext", StringComparison.OrdinalIgnoreCase))
        {
            return ProjectInspectionOutputFormat.PlainText;
        }

        if (string.Equals(value, "json", StringComparison.OrdinalIgnoreCase))
        {
            return ProjectInspectionOutputFormat.Json;
        }

        throw new ArgumentException($"The format '{value}' is not supported. Use 'plain' or 'json'.", nameof(value));
    }

    private ProjectInspectionOutputFormat OutputFormat { get; set; } = ProjectInspectionOutputFormat.PlainText;
}
