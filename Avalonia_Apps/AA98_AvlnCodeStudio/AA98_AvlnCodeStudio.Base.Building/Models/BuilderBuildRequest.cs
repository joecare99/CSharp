namespace AA98_AvlnCodeStudio.Base.Building.Models;

/// <summary>
/// Represents a provider-neutral request to build a project through the builder boundary.
/// </summary>
public sealed class BuilderBuildRequest
{
    /// <summary>
    /// Gets or sets the workspace root path.
    /// </summary>
    public string WorkspaceRootPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the project path to build.
    /// </summary>
    public string ProjectPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional build configuration.
    /// </summary>
    public string? Configuration { get; set; }

    /// <summary>
    /// Gets or sets the optional target framework.
    /// </summary>
    public string? TargetFramework { get; set; }
}
