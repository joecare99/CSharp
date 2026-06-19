using System;
using Workbench.Builder.Core.Models.Inspection;

namespace Workbench.Builder.Host;

/// <summary>
/// Parses command-line arguments for the builder inspection host.
/// </summary>
public sealed class HostCommandLineParser
{
    /// <summary>
    /// Parses the specified command-line arguments.
    /// </summary>
    /// <param name="args">The raw command-line arguments.</param>
    /// <returns>The parsed host command options.</returns>
    public HostCommandOptions Parse(string[] args)
    {
        if (args is null)
        {
            throw new ArgumentNullException(nameof(args));
        }

        string? projectFilePath = null;
        ProjectInspectionOutputFormat outputFormat = ProjectInspectionOutputFormat.PlainText;
        bool showHelp = false;

        for (int i = 0; i < args.Length; i++)
        {
            string argument = args[i];

            if (string.Equals(argument, HostArgumentNames.HelpShort, StringComparison.OrdinalIgnoreCase)
                || string.Equals(argument, HostArgumentNames.HelpLong, StringComparison.OrdinalIgnoreCase))
            {
                showHelp = true;
                continue;
            }

            if (string.Equals(argument, HostArgumentNames.Format, StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length)
                {
                    throw new ArgumentException("A format value is required after --format.", nameof(args));
                }

                outputFormat = ParseOutputFormat(args[++i]);
                continue;
            }

            if (projectFilePath is null)
            {
                projectFilePath = argument;
                continue;
            }

            throw new ArgumentException($"Unexpected argument '{argument}'.", nameof(args));
        }

        return new HostCommandOptions(projectFilePath, outputFormat, showHelp);
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
}
