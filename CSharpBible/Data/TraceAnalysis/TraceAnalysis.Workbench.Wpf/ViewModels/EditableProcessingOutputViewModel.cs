using TraceAnalysis.Workbench.Core.Models;

namespace TraceAnalysis.Workbench.Wpf.ViewModels;

/// <summary>
/// Represents one editable processing output row.
/// </summary>
public sealed class EditableProcessingOutputViewModel : ViewModelBase
{
    private string _outputRole;
    private string _channelName;
    private string _unitText;

    public EditableProcessingOutputViewModel(ProcessingOutputState output)
    {
        _outputRole = output.OutputRole ?? string.Empty;
        _channelName = output.ChannelName;
        _unitText = output.UnitText ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets the semantic output role.
    /// </summary>
    public string OutputRole
    {
        get => _outputRole;
        set => SetProperty(ref _outputRole, value);
    }

    /// <summary>
    /// Gets or sets the output channel name.
    /// </summary>
    public string ChannelName
    {
        get => _channelName;
        set => SetProperty(ref _channelName, value);
    }

    /// <summary>
    /// Gets or sets the output unit text.
    /// </summary>
    public string UnitText
    {
        get => _unitText;
        set => SetProperty(ref _unitText, value);
    }
}
