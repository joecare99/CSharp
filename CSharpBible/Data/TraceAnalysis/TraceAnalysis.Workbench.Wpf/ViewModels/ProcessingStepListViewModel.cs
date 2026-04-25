using System.Collections.ObjectModel;

namespace TraceAnalysis.Workbench.Wpf.ViewModels;

/// <summary>
/// Represents the step list shown by the processing editor.
/// </summary>
public sealed class ProcessingStepListViewModel : ViewModelBase
{
    private EditableProcessingStepViewModel? _selectedStep;

    public ProcessingStepListViewModel(ObservableCollection<EditableProcessingStepViewModel> steps)
    {
        Steps = steps;
        SelectedStep = Steps.Count > 0 ? Steps[0] : null;
    }

    /// <summary>
    /// Gets the processing steps shown by the editor.
    /// </summary>
    public ObservableCollection<EditableProcessingStepViewModel> Steps { get; }

    /// <summary>
    /// Gets or sets the currently selected processing step.
    /// </summary>
    public EditableProcessingStepViewModel? SelectedStep
    {
        get => _selectedStep;
        set => SetProperty(ref _selectedStep, value);
    }

    /// <summary>
    /// Adds a new editable processing step.
    /// </summary>
    public void AddStep(EditableProcessingStepViewModel step)
    {
        Steps.Add(step);
        SelectedStep = step;
    }

    /// <summary>
    /// Removes the currently selected processing step when possible.
    /// </summary>
    public void RemoveSelectedStep()
    {
        if (_selectedStep == null)
            return;

        var index = Steps.IndexOf(_selectedStep);
        Steps.Remove(_selectedStep);

        if (Steps.Count == 0)
        {
            SelectedStep = null;
            return;
        }

        var nextIndex = index >= Steps.Count ? Steps.Count - 1 : index;
        SelectedStep = Steps[nextIndex];
    }
}
