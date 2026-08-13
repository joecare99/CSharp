using System;
using System.Globalization;

namespace MathLibrary.RenderImage;

/// <summary>
/// Represents a mathematical angle whose internal storage uses radians.
/// </summary>
/// <remarks>
/// <para>
/// The <see cref="Value"/> field stores the raw angle in radians. Helper members provide conversion,
/// normalization, arithmetic, and formatted output.
/// </para>
/// <para>
/// Normalization methods map values into the interval from <c>-?</c> inclusive to <c>+?</c> exclusive.
/// This is useful when comparing directions or accumulating rotational values while keeping the result
/// within a predictable canonical range.
/// </para>
/// </remarks>
public struct Angle
{
    /// <summary>
    /// Gets or sets the raw angle value in radians.
    /// </summary>
    /// <remarks>
    /// This field exposes the underlying storage directly. Positive values rotate counterclockwise,
    /// negative values rotate clockwise, assuming the usual mathematical coordinate system.
    /// </remarks>
    public double Value;

    /// <summary>
    /// Gets or sets the angle in degrees.
    /// </summary>
    /// <value>
    /// The current angle expressed in degrees. Reading converts the internal radian value to degrees,
    /// and writing converts the supplied degree value back to radians.
    /// </value>
    public double AsGrad
    {
        readonly get => Value * 180.0 / Math.PI;
        set => Value = value / 180.0 * Math.PI;
    }

    /// <summary>
    /// Normalizes the specified angle to the canonical interval from <c>-?</c> inclusive to <c>+?</c> exclusive.
    /// </summary>
    /// <param name="w">The angle to normalize.</param>
    /// <returns>
    /// A new <see cref="Angle"/> instance whose <see cref="Value"/> is equivalent to <paramref name="w"/>,
    /// but constrained to the canonical range.
    /// </returns>
    public static Angle Normalize(Angle w)
    {
        var twoPi = 2.0 * Math.PI;
        var v = w.Value - Math.Floor((w.Value + Math.PI) / twoPi) * twoPi;
        return new Angle { Value = v };
    }

    /// <summary>
    /// Normalizes the current angle to the canonical interval from <c>-?</c> inclusive to <c>+?</c> exclusive.
    /// </summary>
    /// <returns>
    /// A normalized <see cref="Angle"/> instance that represents the same direction as the current value.
    /// </returns>
    public Angle Normalize() => Normalize(this);

    /// <summary>
    /// Adds another angle to the current value and returns the normalized result.
    /// </summary>
    /// <param name="w">The angle to add.</param>
    /// <returns>
    /// A new <see cref="Angle"/> representing the sum of the two angles, normalized to the canonical range.
    /// </returns>
    public Angle Sum(Angle w) => Normalize(new Angle { Value = Value + w.Value });

    /// <summary>
    /// Adds another angle to the current value, stores the normalized result in the current instance,
    /// and returns the updated value.
    /// </summary>
    /// <param name="w">The angle to add.</param>
    /// <returns>The updated current instance after normalization.</returns>
    public Angle Add(Angle w) { this = Sum(w); return this; }

    /// <summary>
    /// Subtracts another angle from the current value and returns the normalized result.
    /// </summary>
    /// <param name="w">The angle to subtract.</param>
    /// <returns>
    /// A new <see cref="Angle"/> representing the difference between the two angles, normalized to the canonical range.
    /// </returns>
    public Angle Diff(Angle w) => Normalize(new Angle { Value = Value - w.Value });

    /// <summary>
    /// Subtracts another angle from the current value, stores the normalized result in the current instance,
    /// and returns the updated value.
    /// </summary>
    /// <param name="w">The angle to subtract.</param>
    /// <returns>The updated current instance after normalization.</returns>
    public Angle Subt(Angle w) { this = Diff(w); return this; }

    /// <summary>
    /// Returns a culture-invariant string representation of the angle in degrees.
    /// </summary>
    /// <returns>
    /// A string containing the angle formatted with fixed-point notation followed by the degree symbol.
    /// </returns>
    public override readonly string ToString() => string.Format(CultureInfo.InvariantCulture, "{0:F}°", AsGrad);

    /// <summary>
    /// Converts a raw radian value to an <see cref="Angle"/> instance.
    /// </summary>
    /// <param name="v">The angle value in radians.</param>
    public static implicit operator Angle(double v) => new() { Value = v };

    /// <summary>
    /// Converts an <see cref="Angle"/> instance to its raw radian value.
    /// </summary>
    /// <param name="a">The angle instance to convert.</param>
    public static implicit operator double(Angle a) => a.Value;

    /// <summary>
    /// Represents an angle with the value zero radians.
    /// </summary>
    public static readonly Angle Zero = new() { Value = 0.0 };
}
