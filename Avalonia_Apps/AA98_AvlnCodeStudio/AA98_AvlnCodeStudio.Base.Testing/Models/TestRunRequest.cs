using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.Base.Testing.Models;

/// <summary>
/// Represents a provider-neutral request to execute tests.
/// </summary>
public sealed class TestRunRequest
{
    /// <summary>
    /// Gets or sets the optional workspace root path.
    /// </summary>
    public string? WorkspaceRootPath { get; set; }

    /// <summary>
    /// Gets the logical test targets to include in the run.
    /// </summary>
    public IList<string> Targets { get; } = new List<string>();

    /// <summary>
    /// Gets or sets a value indicating whether code coverage should be collected.
    /// </summary>
    public bool CollectCoverage { get; set; }
}
