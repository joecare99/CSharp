using System;
using System.Globalization;

namespace MathLibrary.RenderImage;

/// <summary>
/// Represents a mutable three-dimensional vector or point with <see cref="double"/> precision.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="TFTriple"/> is used throughout the rendering math layer as a compact carrier for Cartesian
/// coordinates, directions, and vector arithmetic in three dimensions.
/// </para>
/// <para>
/// The type is intentionally mutable and exposes its components directly to keep numerical code concise and fast.
/// Methods whose names end with <c>To</c> modify the current instance, while the remaining arithmetic methods return
/// a new value and leave the current instance unchanged.
/// </para>
/// <para>
/// Angular helper methods such as <see cref="InitDirLen(double, double, double)"/> and <see cref="GDir()"/> use the
/// coordinate conventions already established by the surrounding math types, especially <see cref="TFTuple"/>.
/// Angles are expressed in radians.
/// </para>
/// </remarks>
public struct TFTriple
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
    /// Gets or sets the Z component.
    /// </summary>
    public double Z;

    /// <summary>
    /// Gets or sets a vector component by its zero-based axis index.
    /// </summary>
    /// <param name="idx">
    /// The axis index to access: <c>0</c> for <see cref="X"/>, <c>1</c> for <see cref="Y"/>, or <c>2</c> for
    /// <see cref="Z"/>.
    /// </param>
    /// <value>The component value associated with the specified axis index.</value>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="idx"/> is not one of the supported values <c>0</c>, <c>1</c>, or <c>2</c>.
    /// </exception>
    public double this[int idx]
    {
        readonly get => idx switch { 0 => X, 1 => Y, 2 => Z, _ => throw new ArgumentOutOfRangeException(nameof(idx)) };
        set
        {
            switch (idx)
            {
                case 0: X = value; break;
                case 1: Y = value; break;
                case 2: Z = value; break;
                default: throw new ArgumentOutOfRangeException(nameof(idx));
            }
        }
    }

    /// <summary>
    /// Returns a culture-invariant textual representation of the current vector.
    /// </summary>
    /// <returns>
    /// A string in the form <c>&lt; x; y; z &gt;</c> using fixed-point formatting for each component.
    /// </returns>
    public override string ToString() => string.Format(CultureInfo.InvariantCulture, "< {0:F}; {1:F}; {2:F} >", X, Y, Z);

    /// <summary>
    /// Assigns all three Cartesian components explicitly.
    /// </summary>
    /// <param name="aX">The value to assign to <see cref="X"/>.</param>
    /// <param name="aY">The value to assign to <see cref="Y"/>.</param>
    /// <param name="aZ">The value to assign to <see cref="Z"/>.</param>
    public void Init(double aX, double aY, double aZ)
    {
        X = aX; Y = aY; Z = aZ;
    }

    /// <summary>
    /// Initializes the vector from a length and two angular direction values.
    /// </summary>
    /// <param name="len">The vector length or radius.</param>
    /// <param name="dirZ">
    /// The primary direction angle used to distribute the length between the X axis and the YZ plane.
    /// </param>
    /// <param name="dirX">
    /// The secondary direction angle used inside the YZ plane to split the remaining magnitude between Y and Z.
    /// </param>
    /// <remarks>
    /// This method interprets both direction arguments in radians and follows the trigonometric decomposition encoded
    /// in the implementation. It is intended as the inverse counterpart to direction extraction via <see cref="GDir()"/>.
    /// </remarks>
    public void InitDirLen(double len, double dirZ, double dirX)
    {
        X = Math.Cos(dirZ) * len;
        Y = Math.Sin(dirZ) * len * Math.Cos(dirX);
        Z = Math.Sin(dirZ) * len * Math.Sin(dirX);
    }

    /// <summary>
    /// Initializes the current instance from a two-dimensional tuple projected into one of the principal planes.
    /// </summary>
    /// <param name="tuple">The two-dimensional source tuple.</param>
    /// <param name="plane">
    /// The target plane selector: <c>0</c> maps to the XY plane, <c>1</c> maps to the YZ plane, and <c>2</c> maps to
    /// the ZX plane.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="plane"/> is not one of the supported values <c>0</c>, <c>1</c>, or <c>2</c>.
    /// </exception>
    /// <remarks>
    /// The component not covered by the selected plane is set to <c>0</c>.
    /// </remarks>
    public void InitTuple(in TFTuple tuple, int plane = 0)
    {
        switch (plane)
        {
            case 0: Init(tuple.X, tuple.Y, 0); break;
            case 1: Init(0, tuple.X, tuple.Y); break;
            case 2: Init(tuple.Y, 0, tuple.X); break;
            default: throw new ArgumentOutOfRangeException(nameof(plane));
        }
    }

    /// <summary>
    /// Returns the vector sum of the current instance and another vector.
    /// </summary>
    /// <param name="sum">The vector to add.</param>
    /// <returns>A new vector containing the component-wise sum.</returns>
    public TFTriple Add(in TFTriple sum) => new() { X = X + sum.X, Y = Y + sum.Y, Z = Z + sum.Z };

    /// <summary>
    /// Adds another vector to the current instance in place.
    /// </summary>
    /// <param name="sum">The vector to add.</param>
    /// <returns>The modified current instance after the addition.</returns>
    public TFTriple AddTo(in TFTriple sum) { X += sum.X; Y += sum.Y; Z += sum.Z; return this; }

    /// <summary>
    /// Returns the component-wise difference between the current instance and another vector.
    /// </summary>
    /// <param name="dmin">The vector to subtract.</param>
    /// <returns>A new vector containing the subtraction result.</returns>
    public TFTriple Subt(in TFTriple dmin) => new() { X = X - dmin.X, Y = Y - dmin.Y, Z = Z - dmin.Z };

    /// <summary>
    /// Subtracts another vector from the current instance in place.
    /// </summary>
    /// <param name="dmin">The vector to subtract.</param>
    /// <returns>The modified current instance after the subtraction.</returns>
    public TFTriple SubtTo(in TFTriple dmin) { X -= dmin.X; Y -= dmin.Y; Z -= dmin.Z; return this; }

    /// <summary>
    /// Calculates the scalar product of the current instance and another vector.
    /// </summary>
    /// <param name="fak">The second vector participating in the dot product.</param>
    /// <returns>The dot product <c>X1*X2 + Y1*Y2 + Z1*Z2</c>.</returns>
    public double Mul(in TFTriple fak) => X * fak.X + Y * fak.Y + Z * fak.Z;

    /// <summary>
    /// Returns a new vector scaled by the specified factor.
    /// </summary>
    /// <param name="fak">The scalar factor.</param>
    /// <returns>A new vector whose components are multiplied by <paramref name="fak"/>.</returns>
    public TFTriple Mul(double fak) => new() { X = X * fak, Y = Y * fak, Z = Z * fak };

    /// <summary>
    /// Scales the current instance in place.
    /// </summary>
    /// <param name="fak">The scalar factor.</param>
    /// <returns>The modified current instance after scaling.</returns>
    public TFTriple MulTo(double fak) { X *= fak; Y *= fak; Z *= fak; return this; }

    /// <summary>
    /// Returns a new vector divided by the specified scalar value.
    /// </summary>
    /// <param name="divs">The divisor applied to all three components.</param>
    /// <returns>A new vector whose components are divided by <paramref name="divs"/>.</returns>
    /// <remarks>
    /// No explicit zero check is performed. IEEE floating-point rules therefore determine the result for zero or
    /// non-finite divisors.
    /// </remarks>
    public TFTriple Divide(double divs) => new() { X = X / divs, Y = Y / divs, Z = Z / divs };

    /// <summary>
    /// Calculates the vector cross product of the current instance and another vector.
    /// </summary>
    /// <param name="fak">The second vector participating in the cross product.</param>
    /// <returns>
    /// A new vector perpendicular to both operands whose direction follows the right-hand rule.
    /// </returns>
    public TFTriple XMul(in TFTriple fak) => new()
    {
        X = Y * fak.Z - Z * fak.Y,
        Y = Z * fak.X - X * fak.Z,
        Z = X * fak.Y - Y * fak.X
    };

    /// <summary>
    /// Compares the current instance with another vector using an absolute tolerance per component.
    /// </summary>
    /// <param name="probe">The vector to compare with the current instance.</param>
    /// <param name="eps">
    /// The exclusive tolerance threshold applied independently to the absolute difference of each component.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if all three absolute component differences are smaller than <paramref name="eps"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool Equals(in TFTriple probe, double eps = 1e-15) =>
Math.Abs(probe.X - X) < eps && Math.Abs(probe.Y - Y) < eps && Math.Abs(probe.Z - Z) < eps;

    /// <summary>
    /// Returns a normalized copy of the current vector.
    /// </summary>
    /// <returns>A unit-length vector pointing in the same direction as the current instance.</returns>
    /// <remarks>
    /// This method divides the vector by <see cref="GLen()"/> and does not guard against a zero-length input.
    /// </remarks>
    public TFTriple Normalize() => this / GLen();

    /// <summary>
    /// Creates a new vector from explicit Cartesian component values.
    /// </summary>
    /// <param name="nx">The X component.</param>
    /// <param name="ny">The Y component.</param>
    /// <param name="nz">The Z component.</param>
    /// <returns>A new <see cref="TFTriple"/> initialized with the supplied component values.</returns>
    public static TFTriple Copy(double nx, double ny, double nz) => new() { X = nx, Y = ny, Z = nz };

    /// <summary>
    /// Creates a copy of another vector.
    /// </summary>
    /// <param name="vect">The vector to copy.</param>
    /// <returns>A value copy of <paramref name="vect"/>.</returns>
    public static TFTriple Copy(in TFTriple vect) => vect;

    /// <summary>
    /// Creates a value copy of the current instance.
    /// </summary>
    /// <returns>A copy of the current vector.</returns>
    public TFTriple Copy() => this;

    /// <summary>
    /// Calculates the Euclidean length of the vector.
    /// </summary>
    /// <returns>The geometric vector magnitude.</returns>
    public double GLen() => Math.Sqrt(X * X + Y * Y + Z * Z);

    /// <summary>
    /// Calculates the maximum norm of the vector.
    /// </summary>
    /// <returns>The largest absolute component value.</returns>
    public double MLen() => Math.Max(Math.Abs(X), Math.Max(Math.Abs(Y), Math.Abs(Z)));

    /// <summary>
    /// Converts the current vector direction into the angular tuple representation used by <see cref="TFTuple"/>.
    /// </summary>
    /// <returns>
    /// A tuple describing the direction of the current vector. For vectors on the X axis, the result is handled as a
    /// special case to avoid unnecessary secondary angle calculations.
    /// </returns>
    /// <remarks>
    /// The first angle is derived from the relation between the X component and the YZ-plane magnitude, while the
    /// second angle is derived from the relation between Y and Z.
    /// </remarks>
    public TFTuple GDir()
    {
        if (Y == 0.0 && Z == 0.0)
        {
            return X >= 0.0 ? TFTuple.Zero : TFTuple.Copy(Math.PI, 0.0);
        }
        else
        {
            var t0 = new TFTuple { X = X, Y = Math.Sqrt(Y * Y + Z * Z) }.GDir();
            var t1 = new TFTuple { X = Y, Y = Z }.GDir();
            return TFTuple.Copy(t0, t1);
        }
    }

    /// <summary>
    /// Adds two vectors component-wise.
    /// </summary>
    /// <param name="a">The left operand.</param>
    /// <param name="b">The right operand.</param>
    /// <returns>The sum of <paramref name="a"/> and <paramref name="b"/>.</returns>
    public static TFTriple operator +(in TFTriple a, in TFTriple b) => a.Add(b);

    /// <summary>
    /// Subtracts one vector from another component-wise.
    /// </summary>
    /// <param name="a">The left operand.</param>
    /// <param name="b">The right operand to subtract from <paramref name="a"/>.</param>
    /// <returns>The difference of <paramref name="a"/> and <paramref name="b"/>.</returns>
    public static TFTriple operator -(in TFTriple a, in TFTriple b) => a.Subt(b);

    /// <summary>
    /// Negates all vector components.
    /// </summary>
    /// <param name="a">The vector to negate.</param>
    /// <returns>A vector pointing in the opposite direction.</returns>
    public static TFTriple operator -(in TFTriple a) => a.Mul(-1);

    /// <summary>
    /// Calculates the dot product of two vectors.
    /// </summary>
    /// <param name="a">The left operand.</param>
    /// <param name="b">The right operand.</param>
    /// <returns>The scalar dot product.</returns>
    public static double operator *(in TFTriple a, in TFTriple b) => a.Mul(b);

    /// <summary>
    /// Multiplies a vector by a scalar.
    /// </summary>
    /// <param name="a">The vector operand.</param>
    /// <param name="k">The scalar factor.</param>
    /// <returns>The scaled vector.</returns>
    public static TFTriple operator *(in TFTriple a, double k) => a.Mul(k);

    /// <summary>
    /// Multiplies a vector by a scalar.
    /// </summary>
    /// <param name="k">The scalar factor.</param>
    /// <param name="a">The vector operand.</param>
    /// <returns>The scaled vector.</returns>
    public static TFTriple operator *(double k, in TFTriple a) => a.Mul(k);

    /// <summary>
    /// Divides a vector by a scalar.
    /// </summary>
    /// <param name="a">The vector operand.</param>
    /// <param name="k">The scalar divisor.</param>
    /// <returns>The scaled vector after division.</returns>
    public static TFTriple operator /(in TFTriple a, double k) => a.Divide(k);

    /// <summary>
    /// Returns the Euclidean length of the specified vector.
    /// </summary>
    /// <param name="a">The vector whose magnitude should be calculated.</param>
    /// <returns>The geometric length of <paramref name="a"/>.</returns>
    public static double Abs(TFTriple a) => a.GLen();

    /// <summary>
    /// Returns the squared Euclidean length of the specified vector.
    /// </summary>
    /// <param name="a">The vector whose squared magnitude should be calculated.</param>
    /// <returns>The value <c>X² + Y² + Z²</c> for <paramref name="a"/>.</returns>
    public static double Sqr(TFTriple a) => a.X * a.X + a.Y * a.Y + a.Z * a.Z;

    /// <summary>
    /// Represents the zero vector <c>(0, 0, 0)</c>.
    /// </summary>
    public static readonly TFTriple Zero = new() { X = 0.0, Y = 0.0, Z = 0.0 };
}
