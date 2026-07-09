using Workbench.Builder.Cli;

namespace Workbench.Builder.Host;

/// <summary>
/// Represents the parsed command-line options for the builder compile host.
/// </summary>
public sealed class HostCommandOptions : ProjectCommandOptionsBase
{
    /// <summary>
    /// Initializes a new instance of <see cref="HostCommandOptions"/>.
    /// </summary>
    /// <param name="projectFilePath">The project file path to compile.</param>
    /// <param name="outputDirectory">The optional output directory for emitted artifacts.</param>
    /// <param name="verbosity">The requested host verbosity.</param>
    /// <param name="showHelp">A value indicating whether help output was requested.</param>
    public HostCommandOptions(
        string? projectFilePath,
        string? outputDirectory,
        HostVerbosity verbosity,
        bool showHelp)
        : base(projectFilePath, showHelp)
    {
        OutputDirectory = outputDirectory;
        Verbosity = verbosity;
    }

    /// <summary>
    /// Gets the optional output directory for emitted artifacts.
    /// </summary>
    public string? OutputDirectory { get; }

    /// <summary>
    /// Gets the requested host verbosity.
    /// </summary>
    public HostVerbosity Verbosity { get; }
}
