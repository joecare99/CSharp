using System.Collections.ObjectModel;
using TraceAnalysis.Workbench.Core.Models;

namespace TraceAnalysis.Workbench.Wpf.ViewModels;

/// <summary>
/// Represents the diagnostics surface shown by the workbench shell.
/// </summary>
public sealed class DiagnosticsPanelViewModel
{
    public DiagnosticsPanelViewModel(
        ObservableCollection<ValidationIssue> sourceIssues,
        ObservableCollection<ValidationIssue> validationIssues)
    {
        SourceIssues = sourceIssues;
        ValidationIssues = validationIssues;
    }

    /// <summary>
    /// Gets source-related issues.
    /// </summary>
    public ObservableCollection<ValidationIssue> SourceIssues { get; }

    /// <summary>
    /// Gets processing validation issues.
    /// </summary>
    public ObservableCollection<ValidationIssue> ValidationIssues { get; }
}
