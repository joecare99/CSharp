using System;
using System.Globalization;

namespace MathLibrary.RenderImage
{
    public struct TFTriple
    {
        public double X;
        public double Y;
        public double Z;

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

        public override string ToString() => string.Format(CultureInfo.InvariantCulture, "< {0:F}; {1:F}; {2:F} >", X, Y, Z);

        public void Init(double aX, double aY, double aZ)
        {
            X = aX; Y = aY; Z = aZ;
        }

        public void InitDirLen(double len, double dirZ, double dirX)
        {
            X = Math.Cos(dirZ) * len;
            Y = Math.Sin(dirZ) * len * Math.Cos(dirX);
            Z = Math.Sin(dirZ) * len * Math.Sin(dirX);
        }

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

        public TFTriple Add(in TFTriple sum) => new() { X = X + sum.X, Y = Y + sum.Y, Z = Z + sum.Z };
        public TFTriple AddTo(in TFTriple sum) { X += sum.X; Y += sum.Y; Z += sum.Z; return this; }
        public TFTriple Subt(in TFTriple dmin) => new() { X = X - dmin.X, Y = Y - dmin.Y, Z = Z - dmin.Z };
        public TFTriple SubtTo(in TFTriple dmin) { X -= dmin.X; Y -= dmin.Y; Z -= dmin.Z; return this; }

        public double Mul(in TFTriple fak) => X * fak.X + Y * fak.Y + Z * fak.Z;
        public TFTriple Mul(double fak) => new() { X = X * fak, Y = Y * fak, Z = Z * fak };
        public TFTriple MulTo(double fak) { X *= fak; Y *= fak; Z *= fak; return this; }
        public TFTriple Divide(double divs) => new() { X = X / divs, Y = Y / divs, Z = Z / divs };

        public TFTriple XMul(in TFTriple fak) => new()
        {
            X = Y * fak.Z - Z * fak.Y,
            Y = Z * fak.X - X * fak.Z,
            Z = X * fak.Y - Y * fak.X
        };

        public bool Equals(in TFTriple probe, double eps = 1e-15) =>
 Math.Abs(probe.X - X) < eps && Math.Abs(probe.Y - Y) < eps && Math.Abs(probe.Z - Z) < eps;

        public TFTriple Normalize() => this / GLen();

        public static TFTriple Copy(double nx, double ny, double nz) => new() { X = nx, Y = ny, Z = nz };
        public static TFTriple Copy(in TFTriple vect) => vect;
        public TFTriple Copy() => this;

        public double GLen() => Math.Sqrt(X * X + Y * Y + Z * Z);
        public double MLen() => Math.Max(Math.Abs(X), Math.Max(Math.Abs(Y), Math.Abs(Z)));

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

        public static TFTriple operator +(in TFTriple a, in TFTriple b) => a.Add(b);
        public static TFTriple operator -(in TFTriple a, in TFTriple b) => a.Subt(b);
        public static TFTriple operator -(in TFTriple a) => a.Mul(-1);
        public static double operator *(in TFTriple a, in TFTriple b) => a.Mul(b);
        public static TFTriple operator *(in TFTriple a, double k) => a.Mul(k);
        public static TFTriple operator *(double k, in TFTriple a) => a.Mul(k);
        public static TFTriple operator /(in TFTriple a, double k) => a.Divide(k);

        public static double Abs(TFTriple a) => a.GLen();
        public static double Sqr(TFTriple a) => a.X * a.X + a.Y * a.Y + a.Z * a.Z;

        public static readonly TFTriple Zero = new() { X = 0.0, Y = 0.0, Z = 0.0 };
    }
}
