namespace Workbench.Builder.Core.Models.Loading;

/// <summary>
/// Represents the caller-provided input used to evaluate a project for inspection or build preparation.
/// </summary>
public sealed class ProjectLoadRequest
{
    /// <summary>
    /// Initializes a new instance of <see cref="ProjectLoadRequest"/>.
    /// </summary>
    /// <param name="projectFilePath">The project file path to inspect.</param>
    /// <param name="configuration">The optional build configuration to evaluate.</param>
    /// <param name="targetFramework">The optional target framework to evaluate.</param>
    /// <param name="runtimeIdentifier">The optional runtime identifier to evaluate.</param>
    public ProjectLoadRequest(
        string projectFilePath,
        string? configuration = null,
        string? targetFramework = null,
        string? runtimeIdentifier = null)
    {
        ProjectFilePath = projectFilePath;
        Configuration = configuration;
        TargetFramework = targetFramework;
        RuntimeIdentifier = runtimeIdentifier;
    }

    /// <summary>
    /// Gets the project file path to inspect.
    /// </summary>
    public string ProjectFilePath { get; }

    /// <summary>
    /// Gets the optional build configuration to evaluate.
    /// </summary>
    public string? Configuration { get; }

    /// <summary>
    /// Gets the optional target framework to evaluate.
    /// </summary>
    public string? TargetFramework { get; }

    /// <summary>
    /// Gets the optional runtime identifier to evaluate.
    /// </summary>
    public string? RuntimeIdentifier { get; }
}
