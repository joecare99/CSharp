using System;
using System.Globalization;

namespace MathLibrary.RenderImage
{
    public struct Angle
    {
        public double Value;

        public double AsGrad
        {
            readonly get => Value * 180.0 / Math.PI;
            set => Value = value / 180.0 * Math.PI;
        }

        public static Angle Normalize(Angle w)
        {
            var twoPi = 2.0 * Math.PI;
            var v = w.Value - Math.Floor((w.Value + Math.PI) / twoPi) * twoPi;
            return new Angle { Value = v };
        }

        public Angle Normalize() => Normalize(this);

        public Angle Sum(Angle w) => Normalize(new Angle { Value = Value + w.Value });
        public Angle Add(Angle w) { this = Sum(w); return this; }
        public Angle Diff(Angle w) => Normalize(new Angle { Value = Value - w.Value });
        public Angle Subt(Angle w) { this = Diff(w); return this; }

        public override readonly string ToString() => string.Format(CultureInfo.InvariantCulture, "{0:F}°", AsGrad);

        public static implicit operator Angle(double v) => new() { Value = v };
        public static implicit operator double(Angle a) => a.Value;

        public static readonly Angle Zero = new() { Value = 0.0 };
    }
}
