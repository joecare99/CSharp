using System.Globalization;
using TraceAnalysis.Workbench.Core.Models;

namespace TraceAnalysis.Widgets.Wpf.ViewModels;

/// <summary>
/// Represents formatted statistics for the selected chart segment.
/// </summary>
public sealed class TraceCursorStatisticsViewModel
{
    public TraceCursorStatisticsViewModel(string seriesName, string groupName, TraceCursorStatisticsModel statistics)
    {
        SeriesName = seriesName;
        GroupName = groupName;
        SampleCountText = statistics.SampleCount.ToString(CultureInfo.InvariantCulture);
        MinimumText = statistics.Minimum.ToString("0.###", CultureInfo.InvariantCulture);
        MaximumText = statistics.Maximum.ToString("0.###", CultureInfo.InvariantCulture);
        MeanText = statistics.Mean.ToString("0.###", CultureInfo.InvariantCulture);
        MedianText = statistics.Median.ToString("0.###", CultureInfo.InvariantCulture);
        VarianceText = statistics.Variance.ToString("0.###", CultureInfo.InvariantCulture);
        IntegralText = statistics.Integral.ToString("0.###", CultureInfo.InvariantCulture);
        SlopeText = statistics.Slope.ToString("0.###", CultureInfo.InvariantCulture);
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
    /// Gets the formatted sample count.
    /// </summary>
    public string SampleCountText { get; }

    /// <summary>
    /// Gets the formatted minimum value.
    /// </summary>
    public string MinimumText { get; }

    /// <summary>
    /// Gets the formatted maximum value.
    /// </summary>
    public string MaximumText { get; }

    /// <summary>
    /// Gets the formatted mean value.
    /// </summary>
    public string MeanText { get; }

    /// <summary>
    /// Gets the formatted median value.
    /// </summary>
    public string MedianText { get; }

    /// <summary>
    /// Gets the formatted variance.
    /// </summary>
    public string VarianceText { get; }

    /// <summary>
    /// Gets the formatted integral.
    /// </summary>
    public string IntegralText { get; }

    /// <summary>
    /// Gets the formatted slope.
    /// </summary>
    public string SlopeText { get; }
}
