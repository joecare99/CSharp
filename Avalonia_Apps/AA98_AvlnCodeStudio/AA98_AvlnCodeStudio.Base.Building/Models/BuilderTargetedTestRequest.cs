using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.Base.Building.Models;

/// <summary>
/// Represents a provider-neutral request to execute targeted tests through the builder boundary.
/// </summary>
public sealed class BuilderTargetedTestRequest
{
    /// <summary>
    /// Gets or sets the workspace root path.
    /// </summary>
    public string WorkspaceRootPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the test project path.
    /// </summary>
    public string ProjectPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional target framework.
    /// </summary>
    public string? TargetFramework { get; set; }

    /// <summary>
    /// Gets the targeted test names or identifiers.
    /// </summary>
    public IList<string> Targets { get; } = new List<string>();
}
