using System.Globalization;

namespace TraceAnalysis.Widgets.Wpf.ViewModels;

/// <summary>
/// Represents one cursor-aligned sample value for a visible series.
/// </summary>
public sealed class TraceCursorValueViewModel
{
    public TraceCursorValueViewModel(string seriesName, string groupName, double time, double value, bool isCursorA)
    {
        SeriesName = seriesName;
        GroupName = groupName;
        TimeText = time.ToString("0.###", CultureInfo.InvariantCulture);
        ValueText = value.ToString("0.###", CultureInfo.InvariantCulture);
        CursorLabel = isCursorA ? "A" : "B";
    }

    /// <summary>
    /// Gets the series name.
    /// </summary>
    public string SeriesName { get; }

    /// <summary>
    /// Gets the optional group name.
    /// </summary>
    public string GroupName { get; }

    /// <summary>
    /// Gets the cursor label.
    /// </summary>
    public string CursorLabel { get; }

    /// <summary>
    /// Gets the formatted cursor time.
    /// </summary>
    public string TimeText { get; }

    /// <summary>
    /// Gets the formatted cursor value.
    /// </summary>
    public string ValueText { get; }
}
