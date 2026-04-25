using System.Collections.Generic;

namespace TraceAnalysis.Workbench.Core.Models;

/// <summary>
/// Represents the structural data basis derived from a loaded trace source.
/// </summary>
public sealed class TraceDataBasisModel
{
    /// <summary>
    /// Initializes a new instance of <see cref="TraceDataBasisModel"/>.
    /// </summary>
    /// <param name="timeBaseText">The derived time-base description.</param>
    /// <param name="items">The derived structural field items.</param>
    public TraceDataBasisModel(string timeBaseText, IReadOnlyList<TraceDataBasisItem> items)
    {
        TimeBaseText = timeBaseText;
        Items = items;
    }

    /// <summary>
    /// Gets the derived time-base description.
    /// </summary>
    public string TimeBaseText { get; }

    /// <summary>
    /// Gets the derived structural field items.
    /// </summary>
    public IReadOnlyList<TraceDataBasisItem> Items { get; }
}
