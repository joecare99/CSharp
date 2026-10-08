using System;

namespace MathLibrary.Common;

/// <summary>
/// Provides numeric comparison helpers.
/// </summary>
public static class NumericComparisons
{
    /// <summary>
    /// Returns a relative similarity score for two boxed values, preserving the historical float comparison behavior.
    /// </summary>
    /// <param name="first">The first value.</param>
    /// <param name="second">The second value.</param>
    /// <returns>
    /// A ratio from -1 to 1 for two <see cref="float"/> values, or 1 when other values are equal and 0 otherwise.
    /// </returns>
    /// <remarks>
    /// For floating-point values, the ratio is the smaller absolute magnitude divided by the larger absolute magnitude,
    /// with the sign determined by the values. Two zero values have a score of 1.
    /// </remarks>
    public static float RelativeMatch(object? first, object? second)
    {
        if (first is float firstValue && second is float secondValue)
        {
            if (Math.Abs(firstValue) > Math.Abs(secondValue))
                return secondValue / firstValue;
            if (secondValue != 0f)
                return firstValue / secondValue;
            return firstValue == 0f ? 1f : 0f;
        }

        return Equals(first, second) ? 1f : 0f;
    }
}
