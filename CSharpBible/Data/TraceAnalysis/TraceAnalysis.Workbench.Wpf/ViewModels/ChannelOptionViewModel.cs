namespace TraceAnalysis.Workbench.Wpf.ViewModels;

/// <summary>
/// Represents one selectable channel option for processing-step inputs.
/// </summary>
public sealed class ChannelOptionViewModel
{
    public ChannelOptionViewModel(string channelName, bool isDerived)
    {
        ChannelName = channelName;
        IsDerived = isDerived;
    }

    /// <summary>
    /// Gets the channel name.
    /// </summary>
    public string ChannelName { get; }

    /// <summary>
    /// Gets a value indicating whether the channel is derived.
    /// </summary>
    public bool IsDerived { get; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return IsDerived ? $"{ChannelName} (derived)" : ChannelName;
    }
}
