namespace Workbench.Builder.Cli;

/// <summary>
/// Represents the shared command-line options for project-based hosts.
/// </summary>
public abstract class ProjectCommandOptionsBase
{
    /// <summary>
    /// Initializes a new instance of <see cref="ProjectCommandOptionsBase"/>.
    /// </summary>
    /// <param name="projectFilePath">The project file path to process.</param>
    /// <param name="showHelp">A value indicating whether help output was requested.</param>
    protected ProjectCommandOptionsBase(string? projectFilePath, bool showHelp)
    {
        ProjectFilePath = projectFilePath;
        ShowHelp = showHelp;
    }

    /// <summary>
    /// Gets the project file path to process.
    /// </summary>
    public string? ProjectFilePath { get; }

    /// <summary>
    /// Gets a value indicating whether help output was requested.
    /// </summary>
    public bool ShowHelp { get; }
}
