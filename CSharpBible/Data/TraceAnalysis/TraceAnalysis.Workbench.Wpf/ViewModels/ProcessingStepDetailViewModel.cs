namespace TraceAnalysis.Workbench.Wpf.ViewModels;

/// <summary>
/// Represents the selected processing step details shown by the editor.
/// </summary>
public sealed class ProcessingStepDetailViewModel : ViewModelBase
{
    private EditableProcessingStepViewModel? _currentStep;

    public ProcessingStepDetailViewModel(EditableProcessingStepViewModel? step)
    {
        _currentStep = step;
    }

    /// <summary>
    /// Gets the selected step identifier.
    /// </summary>
    public string StepId => _currentStep?.StepId ?? string.Empty;

    /// <summary>
    /// Gets the selected operation name.
    /// </summary>
    public string OperationName => _currentStep?.OperationName ?? string.Empty;

    /// <summary>
    /// Gets the summarized step inputs.
    /// </summary>
    public string InputSummary
    {
        get => _currentStep?.InputSummary ?? string.Empty;
        set
        {
            RaisePropertyChanged();
        }
    }

    /// <summary>
    /// Gets the editable step inputs.
    /// </summary>
    public System.Collections.ObjectModel.ObservableCollection<EditableProcessingInputViewModel> Inputs =>
        _currentStep?.Inputs ?? [];

    /// <summary>
    /// Gets the editable step parameters.
    /// </summary>
    public System.Collections.ObjectModel.ObservableCollection<EditableProcessingParameterViewModel> Parameters =>
        _currentStep?.Parameters ?? [];

    /// <summary>
    /// Gets the output rows of the selected step.
    /// </summary>
    public System.Collections.ObjectModel.ObservableCollection<EditableProcessingOutputViewModel> Outputs =>
        _currentStep?.Outputs ?? [];

    /// <summary>
    /// Updates the selected editable step.
    /// </summary>
    public void SetStep(EditableProcessingStepViewModel? step)
    {
        _currentStep = step;
        RaisePropertyChanged(nameof(StepId));
        RaisePropertyChanged(nameof(OperationName));
        RaisePropertyChanged(nameof(InputSummary));
        RaisePropertyChanged(nameof(Inputs));
        RaisePropertyChanged(nameof(Parameters));
        RaisePropertyChanged(nameof(Outputs));
    }
}
