namespace AA98_AvlnCodeStudio.Planning.Core.Models;

/// <summary>
/// Represents a provider-neutral request to load planning items.
/// </summary>
public sealed class PlanningReadRequest
{
    /// <summary>
    /// Gets or sets the repository root path that contains the <c>DevOps</c> planning directory.
    /// </summary>
    public string RepositoryRootPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the planning root folder name relative to the repository root.
    /// </summary>
    public string PlanningRootPath { get; set; } = "DevOps";
}
