namespace AA98_AvlnCodeStudio.Base.Building.Models;

/// <summary>
/// Represents a provider-neutral request to inspect a project through the builder boundary.
/// </summary>
public sealed class BuilderProjectInspectionRequest
{
    /// <summary>
    /// Gets or sets the workspace root path.
    /// </summary>
    public string WorkspaceRootPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the project path to inspect.
    /// </summary>
    public string ProjectPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional build configuration to evaluate.
    /// </summary>
    public string? Configuration { get; set; }

    /// <summary>
    /// Gets or sets the optional target framework to evaluate.
    /// </summary>
    public string? TargetFramework { get; set; }
}
