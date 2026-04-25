using TraceAnalysis.Workbench.Core.Models;

namespace TraceAnalysis.Workbench.Wpf.ViewModels;

/// <summary>
/// Represents one editable output row in the processing editor.
/// </summary>
public sealed class ProcessingOutputEditorItemViewModel
{
    public ProcessingOutputEditorItemViewModel(ProcessingOutputState output)
    {
        OutputRole = output.OutputRole ?? string.Empty;
        ChannelName = output.ChannelName;
        UnitText = output.UnitText ?? string.Empty;
    }

    /// <summary>
    /// Gets the output role text.
    /// </summary>
    public string OutputRole { get; }

    /// <summary>
    /// Gets the output channel name.
    /// </summary>
    public string ChannelName { get; }

    /// <summary>
    /// Gets the output unit text.
    /// </summary>
    public string UnitText { get; }
}
