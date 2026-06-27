using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.Base.Building.Models;

/// <summary>
/// Represents the structured result of a targeted test run through the builder boundary.
/// </summary>
public sealed class BuilderTargetedTestResult
{
    /// <summary>
    /// Gets or sets the test project path.
    /// </summary>
    public string ProjectPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the high-level targeted test outcome.
    /// </summary>
    public BuilderTargetedTestOutcome Outcome { get; set; } = BuilderTargetedTestOutcome.Unknown;

    /// <summary>
    /// Gets or sets the total number of targeted tests discovered or executed.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the number of passed targeted tests.
    /// </summary>
    public int PassedCount { get; set; }

    /// <summary>
    /// Gets or sets the number of failed targeted tests.
    /// </summary>
    public int FailedCount { get; set; }

    /// <summary>
    /// Gets or sets the number of skipped targeted tests.
    /// </summary>
    public int SkippedCount { get; set; }

    /// <summary>
    /// Gets the diagnostics produced during the targeted test run.
    /// </summary>
    public IList<BuilderDiagnostic> Diagnostics { get; } = new List<BuilderDiagnostic>();
}
