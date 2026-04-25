using TraceAnalysis.Workbench.Core.Models;

namespace TraceAnalysis.Workbench.Wpf.ViewModels;

/// <summary>
/// Represents one editable processing parameter.
/// </summary>
public sealed class EditableProcessingParameterViewModel : ViewModelBase
{
    private string _name;
    private string _valueText;

    public EditableProcessingParameterViewModel(ProcessingParameterState parameter)
    {
        _name = parameter.Name;
        _valueText = parameter.ValueText;
    }

    /// <summary>
    /// Gets or sets the parameter name.
    /// </summary>
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    /// <summary>
    /// Gets or sets the parameter value text.
    /// </summary>
    public string ValueText
    {
        get => _valueText;
        set => SetProperty(ref _valueText, value);
    }
}
