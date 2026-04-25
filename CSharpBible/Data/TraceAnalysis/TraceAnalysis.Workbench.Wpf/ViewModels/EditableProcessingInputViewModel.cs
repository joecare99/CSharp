using System.Collections.ObjectModel;
using System.Linq;
using TraceAnalysis.Workbench.Core.Models;

namespace TraceAnalysis.Workbench.Wpf.ViewModels;

/// <summary>
/// Represents one editable processing input with selectable channel options.
/// </summary>
public sealed class EditableProcessingInputViewModel : ViewModelBase
{
    private string _sourceKind;
    private string? _sourceStepId;
    private ChannelOptionViewModel? _selectedChannel;

    public EditableProcessingInputViewModel(ProcessingInputState input, ObservableCollection<ChannelOptionViewModel> availableChannels)
    {
        _sourceKind = input.SourceKind;
        _sourceStepId = input.SourceStepId;
        AvailableChannels = availableChannels;
        _selectedChannel = availableChannels.FirstOrDefault(channel => channel.ChannelName == input.ChannelName)
            ?? new ChannelOptionViewModel(input.ChannelName, isDerived: input.SourceStepId != null);

        if (!AvailableChannels.Any(channel => channel.ChannelName == _selectedChannel.ChannelName))
            AvailableChannels.Add(_selectedChannel);
    }

    /// <summary>
    /// Gets or sets the source kind.
    /// </summary>
    public string SourceKind
    {
        get => _sourceKind;
        set => SetProperty(ref _sourceKind, value);
    }

    /// <summary>
    /// Gets or sets the producing step identifier.
    /// </summary>
    public string? SourceStepId
    {
        get => _sourceStepId;
        set => SetProperty(ref _sourceStepId, value);
    }

    /// <summary>
    /// Gets the available channel options.
    /// </summary>
    public ObservableCollection<ChannelOptionViewModel> AvailableChannels { get; }

    /// <summary>
    /// Gets or sets the selected channel.
    /// </summary>
    public ChannelOptionViewModel? SelectedChannel
    {
        get => _selectedChannel;
        set => SetProperty(ref _selectedChannel, value);
    }

    /// <summary>
    /// Creates a core input model from the current editable input state.
    /// </summary>
    public ProcessingInputState ToModel()
    {
        var channelName = SelectedChannel?.ChannelName ?? string.Empty;
        var isDerived = SelectedChannel?.IsDerived == true;
        var sourceKind = isDerived ? "derived" : SourceKind;
        var sourceStepId = isDerived ? SourceStepId : null;

        return new ProcessingInputState(sourceKind, channelName, sourceStepId);
    }
}
