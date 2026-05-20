namespace TraceAnalysis.Workbench.Core.Models;

/// <summary>
/// Represents calculated statistics for a series segment between two cursor positions.
/// </summary>
public sealed class TraceCursorStatisticsModel
{
    /// <summary>
    /// Initializes a new instance of <see cref="TraceCursorStatisticsModel"/>.
    /// </summary>
    /// <param name="sampleCount">Number of included samples.</param>
    /// <param name="minimum">Minimum sample value.</param>
    /// <param name="maximum">Maximum sample value.</param>
    /// <param name="mean">Arithmetic mean value.</param>
    /// <param name="median">Median sample value.</param>
    /// <param name="variance">Population variance across the selected segment.</param>
    /// <param name="integral">Trapezoidal integral across the selected segment.</param>
    /// <param name="slope">Average slope from the first to the last selected sample.</param>
    public TraceCursorStatisticsModel(int sampleCount, double minimum, double maximum, double mean, double median, double variance, double integral, double slope)
    {
        SampleCount = sampleCount;
        Minimum = minimum;
        Maximum = maximum;
        Mean = mean;
        Median = median;
        Variance = variance;
        Integral = integral;
        Slope = slope;
    }

    /// <summary>
    /// Gets the number of included samples.
    /// </summary>
    public int SampleCount { get; }

    /// <summary>
    /// Gets the minimum sample value.
    /// </summary>
    public double Minimum { get; }

    /// <summary>
    /// Gets the maximum sample value.
    /// </summary>
    public double Maximum { get; }

    /// <summary>
    /// Gets the arithmetic mean value.
    /// </summary>
    public double Mean { get; }

    /// <summary>
    /// Gets the median sample value.
    /// </summary>
    public double Median { get; }

    /// <summary>
    /// Gets the population variance across the selected segment.
    /// </summary>
    public double Variance { get; }

    /// <summary>
    /// Gets the trapezoidal integral across the selected segment.
    /// </summary>
    public double Integral { get; }

    /// <summary>
    /// Gets the average slope from the first to the last selected sample.
    /// </summary>
    public double Slope { get; }
}
