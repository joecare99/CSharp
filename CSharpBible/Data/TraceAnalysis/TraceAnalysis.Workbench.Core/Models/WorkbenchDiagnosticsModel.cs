using System.Collections.Generic;

namespace TraceAnalysis.Workbench.Core.Models;

/// <summary>
/// Groups source- and processing-related diagnostics for the workbench shell.
/// </summary>
public sealed class WorkbenchDiagnosticsModel
{
    /// <summary>
    /// Initializes a new instance of <see cref="WorkbenchDiagnosticsModel"/>.
    /// </summary>
    /// <param name="sourceIssues">Issues originating from source parsing or loading.</param>
    /// <param name="validationIssues">Issues originating from processing validation.</param>
    public WorkbenchDiagnosticsModel(
        IReadOnlyList<ValidationIssue> sourceIssues,
        IReadOnlyList<ValidationIssue> validationIssues)
    {
        SourceIssues = sourceIssues;
        ValidationIssues = validationIssues;
    }

    /// <summary>
    /// Gets source-related issues.
    /// </summary>
    public IReadOnlyList<ValidationIssue> SourceIssues { get; }

    /// <summary>
    /// Gets processing validation issues.
    /// </summary>
    public IReadOnlyList<ValidationIssue> ValidationIssues { get; }
}
