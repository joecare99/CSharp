using Workbench.Builder.Cli;
using Workbench.Builder.Core.Models.Inspection;

namespace Workbench.Builder.Analysis;

/// <summary>
/// Represents the parsed command-line options for the builder analysis host.
/// </summary>
public sealed class AnalysisCommandOptions : ProjectCommandOptionsBase
{
    /// <summary>
    /// Initializes a new instance of <see cref="AnalysisCommandOptions"/>.
    /// </summary>
    /// <param name="projectFilePath">The project file path to inspect.</param>
    /// <param name="outputFormat">The requested output format.</param>
    /// <param name="showHelp">A value indicating whether help output was requested.</param>
    public AnalysisCommandOptions(
        string? projectFilePath,
        ProjectInspectionOutputFormat outputFormat,
        bool showHelp)
        : base(projectFilePath, showHelp)
    {
        OutputFormat = outputFormat;
    }

    /// <summary>
    /// Gets the requested output format.
    /// </summary>
    public ProjectInspectionOutputFormat OutputFormat { get; }
}
