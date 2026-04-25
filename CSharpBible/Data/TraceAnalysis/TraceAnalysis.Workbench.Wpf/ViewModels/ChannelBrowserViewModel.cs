using System.Collections.ObjectModel;
using TraceAnalysis.Workbench.Core.Models;

namespace TraceAnalysis.Workbench.Wpf.ViewModels;

/// <summary>
/// Represents the channel browser state shown by the workbench shell.
/// </summary>
public sealed class ChannelBrowserViewModel
{
    public ChannelBrowserViewModel(ObservableCollection<TraceChannelItem> channels)
    {
        Channels = channels;
    }

    /// <summary>
    /// Gets the channels displayed by the browser.
    /// </summary>
    public ObservableCollection<TraceChannelItem> Channels { get; }

    /// <summary>
    /// Replaces the channel list from a derived trace data basis.
    /// </summary>
    public void UpdateFromDataBasis(TraceDataBasisModel? dataBasis)
    {
        Channels.Clear();
        if (dataBasis == null)
            return;

        foreach (var item in dataBasis.Items)
            Channels.Add(new TraceChannelItem(item.ColumnName, item.GroupName, isDerived: false));
    }
}
