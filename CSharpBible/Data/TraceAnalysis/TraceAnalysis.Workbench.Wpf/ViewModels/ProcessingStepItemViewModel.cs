using System.Collections.ObjectModel;
using TraceAnalysis.Workbench.Core.Models;

namespace TraceAnalysis.Workbench.Wpf.ViewModels;

/// <summary>
/// Represents one processing step inside the step list.
/// </summary>
public sealed class ProcessingStepItemViewModel
{
    public ProcessingStepItemViewModel(ProcessingStepState step)
    {
        StepId = step.StepId;
        OperationName = step.OperationName;
        IsEnabled = step.IsEnabled;
        Outputs = new ObservableCollection<ProcessingOutputEditorItemViewModel>();
        foreach (var output in step.Outputs)
            Outputs.Add(new ProcessingOutputEditorItemViewModel(output));
    }

    /// <summary>
    /// Gets the step identifier.
    /// </summary>
    public string StepId { get; }

    /// <summary>
    /// Gets the operation name.
    /// </summary>
    public string OperationName { get; }

    /// <summary>
    /// Gets a value indicating whether the step is enabled.
    /// </summary>
    public bool IsEnabled { get; }

    /// <summary>
    /// Gets the configured outputs.
    /// </summary>
    public ObservableCollection<ProcessingOutputEditorItemViewModel> Outputs { get; }
}
