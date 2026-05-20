using System;
using System.Collections.Generic;
using TraceAnalysis.Workbench.Core.Models;

namespace TraceAnalysis.Workbench.Core.Services;

/// <summary>
/// Creates placeholder shell state for the first workbench baseline.
/// </summary>
public sealed class WorkbenchSessionService : IWorkbenchSessionService
{
    /// <inheritdoc/>
    public WorkbenchSessionModel CreateInitialSession()
    {
        IReadOnlyList<TraceChannelItem> channels =
        [
            new TraceChannelItem("Speed", "Motion", isDerived: false),
            new TraceChannelItem("Angle", "Motion", isDerived: false),
            new TraceChannelItem("StatusWord", "Status", isDerived: false),
            new TraceChannelItem("AngleSin", "Derived", isDerived: true)
        ];

        IReadOnlyList<ProcessingInputState> inputs =
        [
            new ProcessingInputState("source", "Angle", sourceStepId: null)
        ];

        IReadOnlyList<ProcessingParameterState> parameters =
        [
            new ProcessingParameterState("angleUnit", "deg")
        ];

        IReadOnlyList<ProcessingOutputState> outputs =
        [
            new ProcessingOutputState("sin", "AngleSin", "1"),
            new ProcessingOutputState("cos", "AngleCos", "1")
        ];

        IReadOnlyList<ProcessingStepState> steps =
        [
            new ProcessingStepState("step-angle-sincos", "sinCos", isEnabled: true, inputs, parameters, outputs)
        ];

        IReadOnlyList<ValidationIssue> sourceIssues =
        [
            new ValidationIssue(ValidationSeverity.Info, "No source file is loaded yet."),
            new ValidationIssue(ValidationSeverity.Info, "Source parse diagnostics will appear here after loading a trace.")
        ];

        IReadOnlyList<ValidationIssue> validationIssues =
        [
            new ValidationIssue(ValidationSeverity.Info, "Create or load a processing configuration to begin editing."),
            new ValidationIssue(ValidationSeverity.Warning, "Preview rendering is not implemented in the first shell baseline.")
        ];

        return new WorkbenchSessionModel(
            configurationName: "New Processing Configuration",
            traceSource: new TraceSourceState(sourceId: null, parseErrorCount: 0, dataBasis: null, series: Array.Empty<TraceSeriesModel>()),
            channels,
            steps,
            new WorkbenchDiagnosticsModel(sourceIssues, validationIssues));
    }
}
