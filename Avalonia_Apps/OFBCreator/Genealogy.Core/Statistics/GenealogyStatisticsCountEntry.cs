using System;

namespace Genealogy.Statistics;

/// <summary>
/// Represents one ranked statistics entry with a label and a count.
/// </summary>
public sealed class GenealogyStatisticsCountEntry
{
    /// <summary>
    /// Initializes a ranked statistics entry.
    /// </summary>
    /// <param name="label">The normalized display label.</param>
    /// <param name="count">The number of matching occurrences.</param>
    public GenealogyStatisticsCountEntry(string label, int count)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(label);
        Label = label;
        Count = count;
    }

    /// <summary>
    /// Gets the normalized display label.
    /// </summary>
    public string Label { get; }

    /// <summary>
    /// Gets the number of matching occurrences.
    /// </summary>
    public int Count { get; }
}
