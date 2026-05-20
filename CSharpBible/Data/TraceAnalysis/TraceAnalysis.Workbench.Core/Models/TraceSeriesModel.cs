using System.Collections.Generic;

namespace TraceAnalysis.Workbench.Core.Models;

/// <summary>
/// Represents one projected numeric trace series that can be visualized in the workbench.
/// </summary>
public sealed class TraceSeriesModel
{
    /// <summary>
    /// Initializes a new instance of <see cref="TraceSeriesModel"/>.
    /// </summary>
    /// <param name="name">Series display name.</param>
    /// <param name="groupName">Optional group name.</param>
    /// <param name="formatText">Optional display format.</param>
    /// <param name="points">Projected sample points.</param>
    public TraceSeriesModel(string name, string? groupName, string? formatText, IReadOnlyList<TracePointModel> points)
    {
        Name = name;
        GroupName = groupName;
        FormatText = formatText;
        Points = points;
    }

    /// <summary>
    /// Gets the series display name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the optional group name.
    /// </summary>
    public string? GroupName { get; }

    /// <summary>
    /// Gets the optional display format.
    /// </summary>
    public string? FormatText { get; }

    /// <summary>
    /// Gets the projected sample points.
    /// </summary>
    public IReadOnlyList<TracePointModel> Points { get; }
}
