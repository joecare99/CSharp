namespace TraceAnalysis.Workbench.Core.Models;

/// <summary>
/// Describes a source or derived trace channel that can be shown in the workbench shell.
/// </summary>
public sealed class TraceChannelItem
{
    /// <summary>
    /// Initializes a new instance of <see cref="TraceChannelItem"/>.
    /// </summary>
    /// <param name="name">Stable channel name.</param>
    /// <param name="group">Optional channel group.</param>
    /// <param name="isDerived">Indicates whether the channel is derived.</param>
    public TraceChannelItem(string name, string? group, bool isDerived)
    {
        Name = name;
        Group = group;
        IsDerived = isDerived;
    }

    /// <summary>
    /// Gets the stable channel name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the optional group name.
    /// </summary>
    public string? Group { get; }

    /// <summary>
    /// Gets a value indicating whether this is a derived channel.
    /// </summary>
    public bool IsDerived { get; }
}
