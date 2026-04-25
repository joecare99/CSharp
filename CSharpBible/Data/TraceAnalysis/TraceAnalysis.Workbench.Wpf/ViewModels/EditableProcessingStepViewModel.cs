using System.Collections.ObjectModel;
using System.Linq;
using TraceAnalysis.Workbench.Core.Models;

namespace TraceAnalysis.Workbench.Wpf.ViewModels;

/// <summary>
/// Represents one editable processing step in the editor.
/// </summary>
public sealed class EditableProcessingStepViewModel : ViewModelBase
{
    private string _stepId;
    private string _operationName;
    private bool _isEnabled;
    private readonly ObservableCollection<ChannelOptionViewModel> _availableInputChannels;

    public EditableProcessingStepViewModel(ProcessingStepState step, ObservableCollection<ChannelOptionViewModel> availableInputChannels)
    {
        _stepId = step.StepId;
        _operationName = step.OperationName;
        _isEnabled = step.IsEnabled;
        _availableInputChannels = availableInputChannels;

        Inputs = new ObservableCollection<EditableProcessingInputViewModel>();
        foreach (var input in step.Inputs)
            Inputs.Add(new EditableProcessingInputViewModel(input, _availableInputChannels));

        Parameters = new ObservableCollection<EditableProcessingParameterViewModel>();
        foreach (var parameter in step.Parameters)
            Parameters.Add(new EditableProcessingParameterViewModel(parameter));

        Outputs = new ObservableCollection<EditableProcessingOutputViewModel>();
        foreach (var output in step.Outputs)
            Outputs.Add(new EditableProcessingOutputViewModel(output));
    }

    /// <summary>
    /// Gets or sets the step identifier.
    /// </summary>
    public string StepId
    {
        get => _stepId;
        set => SetProperty(ref _stepId, value);
    }

    /// <summary>
    /// Gets or sets the operation name.
    /// </summary>
    public string OperationName
    {
        get => _operationName;
        set => SetProperty(ref _operationName, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the step is enabled.
    /// </summary>
    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }

    /// <summary>
    /// Gets the editable inputs.
    /// </summary>
    public ObservableCollection<EditableProcessingInputViewModel> Inputs { get; }

    /// <summary>
    /// Gets the editable parameters.
    /// </summary>
    public ObservableCollection<EditableProcessingParameterViewModel> Parameters { get; }

    /// <summary>
    /// Gets the editable outputs.
    /// </summary>
    public ObservableCollection<EditableProcessingOutputViewModel> Outputs { get; }

    /// <summary>
    /// Gets a textual summary of the current inputs.
    /// </summary>
    public string InputSummary => string.Join(", ", Inputs.Select(input => input.SelectedChannel?.ChannelName ?? string.Empty).Where(channel => !string.IsNullOrWhiteSpace(channel)));

    /// <summary>
    /// Creates a core model snapshot from the current editable step state.
    /// </summary>
    public ProcessingStepState ToModel()
    {
        var inputs = Inputs
            .Select(input => input.ToModel())
            .ToList();

        var parameters = Parameters
            .Select(parameter => new ProcessingParameterState(parameter.Name, parameter.ValueText))
            .ToList();

        var outputs = Outputs
            .Select(output => new ProcessingOutputState(
                string.IsNullOrWhiteSpace(output.OutputRole) ? null : output.OutputRole,
                output.ChannelName,
                string.IsNullOrWhiteSpace(output.UnitText) ? null : output.UnitText))
            .ToList();

        return new ProcessingStepState(StepId, OperationName, IsEnabled, inputs, parameters, outputs);
    }

    /// <summary>
    /// Adds a new editable input using the first available channel option when possible.
    /// </summary>
    public void AddInput()
    {
        var defaultChannel = _availableInputChannels.FirstOrDefault() ?? new ChannelOptionViewModel(string.Empty, isDerived: false);
        Inputs.Add(new EditableProcessingInputViewModel(new ProcessingInputState("source", defaultChannel.ChannelName, sourceStepId: null), _availableInputChannels));
        RaisePropertyChanged(nameof(InputSummary));
    }
}
