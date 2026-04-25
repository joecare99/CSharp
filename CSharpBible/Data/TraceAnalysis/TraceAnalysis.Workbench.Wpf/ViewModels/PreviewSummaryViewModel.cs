using System.Collections.ObjectModel;

namespace TraceAnalysis.Workbench.Wpf.ViewModels;

/// <summary>
/// Represents a lightweight preview summary for the currently selected step.
/// </summary>
public sealed class PreviewSummaryViewModel : ViewModelBase
{
    private string _titleText;
    private string _descriptionText;

    public PreviewSummaryViewModel()
    {
        _titleText = "No step selected";
        _descriptionText = "Select a processing step to review its expected inputs and outputs.";
        OutputNames = new ObservableCollection<string>();
    }

    /// <summary>
    /// Gets or sets the preview title text.
    /// </summary>
    public string TitleText
    {
        get => _titleText;
        set => SetProperty(ref _titleText, value);
    }

    /// <summary>
    /// Gets or sets the preview description text.
    /// </summary>
    public string DescriptionText
    {
        get => _descriptionText;
        set => SetProperty(ref _descriptionText, value);
    }

    /// <summary>
    /// Gets the expected output names.
    /// </summary>
    public ObservableCollection<string> OutputNames { get; }

    /// <summary>
    /// Updates the preview summary from the selected step.
    /// </summary>
    public void UpdateFromStep(EditableProcessingStepViewModel? step)
    {
        OutputNames.Clear();
        if (step == null)
        {
            TitleText = "No step selected";
            DescriptionText = "Select a processing step to review its expected inputs and outputs.";
            return;
        }

        TitleText = $"Preview for {step.StepId}";
        DescriptionText = $"Operation: {step.OperationName} | Inputs: {step.InputSummary}";

        foreach (var output in step.Outputs)
            OutputNames.Add(string.IsNullOrWhiteSpace(output.OutputRole)
                ? output.ChannelName
                : $"{output.OutputRole}: {output.ChannelName}");
    }
}
