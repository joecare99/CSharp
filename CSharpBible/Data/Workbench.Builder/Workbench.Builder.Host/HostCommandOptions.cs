using Workbench.Builder.Core.Models.Inspection;

namespace Workbench.Builder.Host;

/// <summary>
/// Represents the parsed command-line options for the builder inspection host.
/// </summary>
public sealed class HostCommandOptions
{
    /// <summary>
    /// Initializes a new instance of <see cref="HostCommandOptions"/>.
    /// </summary>
    /// <param name="projectFilePath">The project file path to inspect.</param>
    /// <param name="outputFormat">The requested output format.</param>
    /// <param name="showHelp">A value indicating whether help output was requested.</param>
    public HostCommandOptions(string? projectFilePath, ProjectInspectionOutputFormat outputFormat, bool showHelp)
    {
        ProjectFilePath = projectFilePath;
        OutputFormat = outputFormat;
        ShowHelp = showHelp;
    }

    /// <summary>
    /// Gets the project file path to inspect.
    /// </summary>
    public string? ProjectFilePath { get; }

    /// <summary>
    /// Gets the requested output format.
    /// </summary>
    public ProjectInspectionOutputFormat OutputFormat { get; }

    /// <summary>
    /// Gets a value indicating whether help output was requested.
    /// </summary>
    public bool ShowHelp { get; }
}
