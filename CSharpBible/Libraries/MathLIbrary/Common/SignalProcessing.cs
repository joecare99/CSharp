using System;
using System.Numerics;

namespace MathLibrary.Common;

/// <summary>
/// Provides reusable signal-processing operations for scalar double-precision samples.
/// </summary>
public static class SignalProcessing
{
    /// <summary>
    /// Applies a first-order low-pass filter to an input sample and updates the filter state.
    /// </summary>
    /// <param name="input">The current input sample.</param>
    /// <param name="state">The previous filter output; updated to the new output.</param>
    /// <param name="alpha">The input weight in the inclusive range [0, 1].</param>
    /// <param name="gain">The gain applied to the input sample.</param>
    /// <returns>The filtered output.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="alpha"/> is outside [0, 1] or is not finite.</exception>
    /// <remarks>The recurrence is <c>state = alpha * gain * input + (1 - alpha) * state</c>.</remarks>
    public static double ApplyFirstOrderLowPass(double input, ref double state, double alpha = 0.1d, double gain = 1.0d)
    {
        if (double.IsNaN(alpha) || double.IsInfinity(alpha) || alpha < 0d || alpha > 1d)
            throw new ArgumentOutOfRangeException(nameof(alpha), alpha, "The input weight must be finite and in the inclusive range [0, 1].");

        state = alpha * gain * input + (1d - alpha) * state;
        return state;
    }

    /// <summary>
    /// Applies the historical configurable-quantile operation to a circular buffer.
    /// </summary>
    /// <param name="sample">The sample to add.</param>
    /// <param name="buffer">The non-empty circular buffer containing the current window.</param>
    /// <param name="nextIndex">The index to replace; updated to the next buffer position.</param>
    /// <param name="quantile">The quantile index in the inclusive range [0, 1].</param>
    /// <returns>The selected order statistic.</returns>
    public static double UpdateCircularMedian(double sample, double[] buffer, ref int nextIndex, double quantile = 0.5d)
        => UpdateCircularQuantile(sample, buffer, ref nextIndex, quantile);

    /// <summary>
    /// Replaces the next value in a circular buffer and returns the arithmetic mean of the buffer.
    /// </summary>
    /// <param name="sample">The sample to add.</param>
    /// <param name="buffer">The non-empty circular buffer containing the current window.</param>
    /// <param name="nextIndex">The index to replace; updated to the next buffer position.</param>
    /// <returns>The arithmetic mean of all values in <paramref name="buffer"/> after insertion.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="buffer"/> is empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="nextIndex"/> is not a valid buffer index.</exception>
    public static double UpdateMovingAverage(double sample, double[] buffer, ref int nextIndex)
    {
        ValidateBuffer(buffer, nextIndex);
        buffer[nextIndex] = sample;
        nextIndex = (nextIndex + 1) % buffer.Length;

        double sum = 0d;
        for (int i = 0; i < buffer.Length; i++)
            sum += buffer[i];
        return sum / buffer.Length;
    }

    /// <summary>
    /// Replaces the next value in a circular buffer and returns the statistical median of the buffer.
    /// </summary>
    /// <param name="sample">The sample to add.</param>
    /// <param name="buffer">The non-empty circular buffer containing the current window.</param>
    /// <param name="nextIndex">The index to replace; updated to the next buffer position.</param>
    /// <returns>The middle value, or the mean of the two middle values when the buffer length is even.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="buffer"/> is empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="nextIndex"/> is not a valid buffer index.</exception>
    public static double UpdateMovingMedian(double sample, double[] buffer, ref int nextIndex)
    {
        ValidateBuffer(buffer, nextIndex);
        buffer[nextIndex] = sample;
        nextIndex = (nextIndex + 1) % buffer.Length;

        double[] sorted = (double[])buffer.Clone();
        Array.Sort(sorted);
        int middle = sorted.Length / 2;
        return sorted.Length % 2 == 0
            ? sorted[middle - 1] / 2d + sorted[middle] / 2d
            : sorted[middle];
    }

    /// <summary>
    /// Replaces the next value in a circular buffer and returns the selected quantile value.
    /// </summary>
    /// <param name="sample">The sample to add.</param>
    /// <param name="buffer">The non-empty circular buffer containing the current window.</param>
    /// <param name="nextIndex">The index to replace; updated to the next buffer position.</param>
    /// <param name="quantile">The quantile index in the inclusive range [0, 1].</param>
    /// <returns>The sorted value at index <c>floor(buffer.Length * quantile)</c>, clamped to the last index.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="buffer"/> is empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="nextIndex"/> is invalid or <paramref name="quantile"/> is outside [0, 1].</exception>
    public static double UpdateCircularQuantile(double sample, double[] buffer, ref int nextIndex, double quantile)
    {
        ValidateBuffer(buffer, nextIndex);
        if (double.IsNaN(quantile) || double.IsInfinity(quantile) || quantile < 0d || quantile > 1d)
            throw new ArgumentOutOfRangeException(nameof(quantile), quantile, "The quantile must be finite and in the inclusive range [0, 1].");

        buffer[nextIndex] = sample;
        nextIndex = (nextIndex + 1) % buffer.Length;

        double[] sorted = (double[])buffer.Clone();
        Array.Sort(sorted);
        int index = (int)Math.Floor(sorted.Length * quantile);
        return sorted[Math.Min(index, sorted.Length - 1)];
    }

    /// <summary>
    /// Converts a polar magnitude and phase to a complex number.
    /// </summary>
    /// <param name="magnitude">The magnitude.</param>
    /// <param name="phaseRadians">The phase angle in radians.</param>
    /// <returns>The corresponding complex number.</returns>
    public static Complex MagnitudePhaseToComplex(double magnitude, double phaseRadians)
        => new(magnitude * Math.Cos(phaseRadians), magnitude * Math.Sin(phaseRadians));

    private static void ValidateBuffer(double[] buffer, int nextIndex)
    {
        if (buffer is null)
            throw new ArgumentNullException(nameof(buffer));
        if (buffer.Length == 0)
            throw new ArgumentException("The circular buffer must not be empty.", nameof(buffer));
        if (nextIndex < 0 || nextIndex >= buffer.Length)
            throw new ArgumentOutOfRangeException(nameof(nextIndex), nextIndex, "The index must identify an element in the circular buffer.");
    }
}
