namespace TraceAnalysis.Workbench.Core.Models;

/// <summary>
/// Represents one numeric trace sample point projected onto a time axis.
/// </summary>
public sealed class TracePointModel
{
    /// <summary>
    /// Initializes a new instance of <see cref="TracePointModel"/>.
    /// </summary>
    /// <param name="time">Projected time value on the horizontal axis.</param>
    /// <param name="value">Projected numeric value on the vertical axis.</param>
    public TracePointModel(double time, double value)
    {
        Time = time;
        Value = value;
    }

    /// <summary>
    /// Gets the projected time value.
    /// </summary>
    public double Time { get; }

    /// <summary>
    /// Gets the numeric sample value.
    /// </summary>
    public double Value { get; }
}
