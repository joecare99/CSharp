using System;
using System.Globalization;

namespace MathLibrary.RenderImage
{
    public struct TFTuple
    {
        public double X;
        public double Y;

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

        public override string ToString() => string.Format(CultureInfo.InvariantCulture, "<{0:F};{1:F}>", X, Y);

        public void Init(double aX, double aY)
        {
            X = aX; Y = aY;
        }

        public void InitLenDir(double len, double dir)
        {
            X = Math.Cos(dir) * len;
            Y = Math.Sin(dir) * len;
        }

        public TFTuple Sum(in TFTuple val) => new() { X = X + val.X, Y = Y + val.Y };
        public TFTuple Add(in TFTuple val)
        {
            this = Sum(val);
            return this;
        }
        public TFTuple Subt(in TFTuple dmin) => new() { X = X - dmin.X, Y = Y - dmin.Y };
        public TFTuple SubtTo(in TFTuple dmin)
        {
            X -= dmin.X; Y -= dmin.Y; return this;
        }

        public double Mul(in TFTuple fak) => X * fak.X + Y * fak.Y; // dot product
        public TFTuple Mul(double fak) => new() { X = X * fak, Y = Y * fak };
        public TFTuple MulTo(double fak) { X *= fak; Y *= fak; return this; }
        public TFTuple Divide(double divs) => new() { X = X / divs, Y = Y / divs };

        // Complex-like multiplication (VMul)
        public TFTuple VMul(in TFTuple fak) => new() { X = X * fak.X - Y * fak.Y, Y = X * fak.Y + Y * fak.X };

        public bool Equals(in TFTuple probe, double eps = 1e-15) => Math.Abs(probe.X - X) < eps && Math.Abs(probe.Y - Y) < eps;

        public static TFTuple Copy(double nx, double ny) => new() { X = nx, Y = ny };
        public static TFTuple Copy(in TFTuple vect) => vect;
        public TFTuple Copy() => this;

        public double GLen() => Math.Sqrt(X * X + Y * Y);
        public double MLen() => Math.Max(Math.Abs(X), Math.Abs(Y));

        public double GDir() => Math.Atan2(Y, X);

        public static TFTuple operator +(in TFTuple a, in TFTuple b) => a.Sum(b);
        public static TFTuple operator -(in TFTuple a, in TFTuple b) => a.Subt(b);
        public static TFTuple operator -(in TFTuple a) => a.Mul(-1);
        public static double operator *(in TFTuple a, in TFTuple b) => a.Mul(b);
        public static TFTuple operator *(in TFTuple a, double k) => a.Mul(k);
        public static TFTuple operator *(double k, in TFTuple a) => a.Mul(k);
        public static TFTuple operator /(in TFTuple a, double k) => a.Divide(k);

        public static double Abs(in TFTuple a) => a.GLen();
        public static double Sqr(in TFTuple a) => a.X * a.X + a.Y * a.Y;

        public static readonly TFTuple Zero = new() { X = 0.0, Y = 0.0 };
    }
}
