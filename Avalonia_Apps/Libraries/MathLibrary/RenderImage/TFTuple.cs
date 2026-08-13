using System;
using System.Globalization;

namespace MathLibrary.RenderImage;

/// <summary>
/// Represents a mutable two-dimensional vector or point with <see cref="double"/> precision.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="TFTuple"/> serves as a compact math helper for planar coordinates, directions, and vector arithmetic
/// within the rendering-related geometry layer.
/// </para>
/// <para>
/// The type is intentionally mutable and exposes its components directly for concise numeric code. Methods whose
/// names end with <c>To</c> modify the current instance, while the remaining arithmetic methods return a new value
/// and leave the current instance unchanged.
/// </para>
/// <para>
/// Angular methods such as <see cref="InitLenDir(double, double)"/> and <see cref="GDir()"/> operate on radians and
/// follow the Cartesian conventions used by the surrounding math types.
/// </para>
/// </remarks>
public struct TFTuple
{
    /// <summary>
    /// Gets or sets the X component.
    /// </summary>
    public double X;

    /// <summary>
    /// Gets or sets the Y component.
    /// </summary>
    public double Y;

    /// <summary>
    /// Gets or sets a component by its zero-based axis index.
    /// </summary>
    /// <param name="idx">
    /// The axis index to access: <c>0</c> for <see cref="X"/> or <c>1</c> for <see cref="Y"/>.
    /// </param>
    /// <value>The component value associated with the specified axis index.</value>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="idx"/> is not one of the supported values <c>0</c> or <c>1</c>.
    /// </exception>
    public double this[int idx]
    {
        readonly get => idx switch { 0 => X, 1 => Y, _ => throw new ArgumentOutOfRangeException(nameof(idx)) };
        set
        {
            switch (idx)
            {
                case 0: X = value; break;
                case 1: Y = value; break;
                default: throw new ArgumentOutOfRangeException(nameof(idx));
            }
        }
    }

    /// <summary>
    /// Returns a culture-invariant textual representation of the current tuple.
    /// </summary>
    /// <returns>
    /// A string in the form <c>&lt;x;y&gt;</c> using fixed-point formatting for both components.
    /// </returns>
    public override string ToString() => string.Format(CultureInfo.InvariantCulture, "<{0:F};{1:F}>", X, Y);

    /// <summary>
    /// Assigns both Cartesian components explicitly.
    /// </summary>
    /// <param name="aX">The value to assign to <see cref="X"/>.</param>
    /// <param name="aY">The value to assign to <see cref="Y"/>.</param>
    public void Init(double aX, double aY)
    {
        X = aX; Y = aY;
    }

    /// <summary>
    /// Initializes the tuple from a length and direction angle.
    /// </summary>
    /// <param name="len">The vector length or radius.</param>
    /// <param name="dir">The direction angle in radians.</param>
    /// <remarks>
    /// The method converts polar coordinates into Cartesian coordinates using cosine for <see cref="X"/> and sine
    /// for <see cref="Y"/>.
    /// </remarks>
    public void InitLenDir(double len, double dir)
    {
        X = Math.Cos(dir) * len;
        Y = Math.Sin(dir) * len;
    }

    /// <summary>
    /// Returns the vector sum of the current instance and another tuple.
    /// </summary>
    /// <param name="val">The tuple to add.</param>
    /// <returns>A new tuple containing the component-wise sum.</returns>
    public TFTuple Sum(in TFTuple val) => new() { X = X + val.X, Y = Y + val.Y };

    /// <summary>
    /// Adds another tuple to the current instance in place.
    /// </summary>
    /// <param name="val">The tuple to add.</param>
    /// <returns>The modified current instance after the addition.</returns>
    public TFTuple Add(in TFTuple val)
    {
        this = Sum(val);
        return this;
    }

    /// <summary>
    /// Returns the component-wise difference between the current instance and another tuple.
    /// </summary>
    /// <param name="dmin">The tuple to subtract.</param>
    /// <returns>A new tuple containing the subtraction result.</returns>
    public TFTuple Subt(in TFTuple dmin) => new() { X = X - dmin.X, Y = Y - dmin.Y };

    /// <summary>
    /// Subtracts another tuple from the current instance in place.
    /// </summary>
    /// <param name="dmin">The tuple to subtract.</param>
    /// <returns>The modified current instance after the subtraction.</returns>
    public TFTuple SubtTo(in TFTuple dmin)
    {
        X -= dmin.X; Y -= dmin.Y; return this;
    }

    /// <summary>
    /// Calculates the scalar product of the current instance and another tuple.
    /// </summary>
    /// <param name="fak">The second tuple participating in the dot product.</param>
    /// <returns>The dot product <c>X1*X2 + Y1*Y2</c>.</returns>
    public double Mul(in TFTuple fak) => X * fak.X + Y * fak.Y; // dot product

    /// <summary>
    /// Returns a new tuple scaled by the specified factor.
    /// </summary>
    /// <param name="fak">The scalar factor.</param>
    /// <returns>A new tuple whose components are multiplied by <paramref name="fak"/>.</returns>
    public TFTuple Mul(double fak) => new() { X = X * fak, Y = Y * fak };

    /// <summary>
    /// Scales the current instance in place.
    /// </summary>
    /// <param name="fak">The scalar factor.</param>
    /// <returns>The modified current instance after scaling.</returns>
    public TFTuple MulTo(double fak) { X *= fak; Y *= fak; return this; }

    /// <summary>
    /// Returns a new tuple divided by the specified scalar value.
    /// </summary>
    /// <param name="divs">The divisor applied to both components.</param>
    /// <returns>A new tuple whose components are divided by <paramref name="divs"/>.</returns>
    /// <remarks>
    /// No explicit zero check is performed. IEEE floating-point rules therefore determine the result for zero or
    /// non-finite divisors.
    /// </remarks>
    public TFTuple Divide(double divs) => new() { X = X / divs, Y = Y / divs };

    /// <summary>
    /// Calculates a complex-number-like multiplication of two tuples.
    /// </summary>
    /// <param name="fak">The second tuple participating in the multiplication.</param>
    /// <returns>
    /// A new tuple with the result <c>(X1*X2 - Y1*Y2, X1*Y2 + Y1*X2)</c>.
    /// </returns>
    /// <remarks>
    /// This operation matches multiplication in the complex plane when <see cref="X"/> is interpreted as the real
    /// part and <see cref="Y"/> as the imaginary part.
    /// </remarks>
    public TFTuple VMul(in TFTuple fak) => new() { X = X * fak.X - Y * fak.Y, Y = X * fak.Y + Y * fak.X };

    /// <summary>
    /// Compares the current instance with another tuple using an absolute tolerance per component.
    /// </summary>
    /// <param name="probe">The tuple to compare with the current instance.</param>
    /// <param name="eps">
    /// The exclusive tolerance threshold applied independently to the absolute difference of each component.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if all absolute component differences are smaller than <paramref name="eps"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool Equals(in TFTuple probe, double eps = 1e-15) => Math.Abs(probe.X - X) < eps && Math.Abs(probe.Y - Y) < eps;

    /// <summary>
    /// Creates a new tuple from explicit Cartesian component values.
    /// </summary>
    /// <param name="nx">The X component.</param>
    /// <param name="ny">The Y component.</param>
    /// <returns>A new <see cref="TFTuple"/> initialized with the supplied component values.</returns>
    public static TFTuple Copy(double nx, double ny) => new() { X = nx, Y = ny };

    /// <summary>
    /// Creates a copy of another tuple.
    /// </summary>
    /// <param name="vect">The tuple to copy.</param>
    /// <returns>A value copy of <paramref name="vect"/>.</returns>
    public static TFTuple Copy(in TFTuple vect) => vect;

    /// <summary>
    /// Creates a value copy of the current instance.
    /// </summary>
    /// <returns>A copy of the current tuple.</returns>
    public TFTuple Copy() => this;

    /// <summary>
    /// Calculates the Euclidean length of the tuple.
    /// </summary>
    /// <returns>The geometric vector magnitude.</returns>
    public double GLen() => Math.Sqrt(X * X + Y * Y);

    /// <summary>
    /// Calculates the maximum norm of the tuple.
    /// </summary>
    /// <returns>The largest absolute component value.</returns>
    public double MLen() => Math.Max(Math.Abs(X), Math.Abs(Y));

    /// <summary>
    /// Returns the geometric direction angle of the tuple.
    /// </summary>
    /// <returns>
    /// The angle in radians measured from the positive X axis toward the tuple, as computed by <see cref="Math.Atan2(double, double)"/>.
    /// </returns>
    public double GDir() => Math.Atan2(Y, X);

    /// <summary>
    /// Adds two tuples component-wise.
    /// </summary>
    /// <param name="a">The left operand.</param>
    /// <param name="b">The right operand.</param>
    /// <returns>The sum of <paramref name="a"/> and <paramref name="b"/>.</returns>
    public static TFTuple operator +(in TFTuple a, in TFTuple b) => a.Sum(b);

    /// <summary>
    /// Subtracts one tuple from another component-wise.
    /// </summary>
    /// <param name="a">The left operand.</param>
    /// <param name="b">The right operand to subtract from <paramref name="a"/>.</param>
    /// <returns>The difference of <paramref name="a"/> and <paramref name="b"/>.</returns>
    public static TFTuple operator -(in TFTuple a, in TFTuple b) => a.Subt(b);

    /// <summary>
    /// Negates both tuple components.
    /// </summary>
    /// <param name="a">The tuple to negate.</param>
    /// <returns>A tuple pointing in the opposite direction.</returns>
    public static TFTuple operator -(in TFTuple a) => a.Mul(-1);

    /// <summary>
    /// Calculates the dot product of two tuples.
    /// </summary>
    /// <param name="a">The left operand.</param>
    /// <param name="b">The right operand.</param>
    /// <returns>The scalar dot product.</returns>
    public static double operator *(in TFTuple a, in TFTuple b) => a.Mul(b);

    /// <summary>
    /// Multiplies a tuple by a scalar.
    /// </summary>
    /// <param name="a">The tuple operand.</param>
    /// <param name="k">The scalar factor.</param>
    /// <returns>The scaled tuple.</returns>
    public static TFTuple operator *(in TFTuple a, double k) => a.Mul(k);

    /// <summary>
    /// Multiplies a tuple by a scalar.
    /// </summary>
    /// <param name="k">The scalar factor.</param>
    /// <param name="a">The tuple operand.</param>
    /// <returns>The scaled tuple.</returns>
    public static TFTuple operator *(double k, in TFTuple a) => a.Mul(k);

    /// <summary>
    /// Divides a tuple by a scalar.
    /// </summary>
    /// <param name="a">The tuple operand.</param>
    /// <param name="k">The scalar divisor.</param>
    /// <returns>The scaled tuple after division.</returns>
    public static TFTuple operator /(in TFTuple a, double k) => a.Divide(k);

    /// <summary>
    /// Returns the Euclidean length of the specified tuple.
    /// </summary>
    /// <param name="a">The tuple whose magnitude should be calculated.</param>
    /// <returns>The geometric length of <paramref name="a"/>.</returns>
    public static double Abs(in TFTuple a) => a.GLen();

    /// <summary>
    /// Returns the squared Euclidean length of the specified tuple.
    /// </summary>
    /// <param name="a">The tuple whose squared magnitude should be calculated.</param>
    /// <returns>The value <c>X² + Y²</c> for <paramref name="a"/>.</returns>
    public static double Sqr(in TFTuple a) => a.X * a.X + a.Y * a.Y;

    /// <summary>
    /// Represents the zero tuple <c>(0, 0)</c>.
    /// </summary>
    public static readonly TFTuple Zero = new() { X = 0.0, Y = 0.0 };
}
