using System;
using System.Globalization;

namespace RenderImage.Base.Model
{
    public struct RenderColor
    {
        public double Red;
        public double Green;
        public double Blue;

        public void Init(double red, double green, double blue)
        {
            Red = red; Green = green; Blue = blue;
        }

        public void InitHSB(double hue, double satur, double bright)
        {
            var sat = Math.Min(1.0 - Math.Abs(1.0 - bright * 2.0), satur) * 0.5;
            Red = Math.Cos(2 * Math.PI * hue) * sat + bright;
            Green = Math.Cos(2 * Math.PI * (hue - 1.0 / 3.0)) * sat + bright;
            Blue = Math.Cos(2 * Math.PI * (hue - 2.0 / 3.0)) * sat + bright;
        }

        public RenderColor Plus(RenderColor c) => new() { Red = Red + c.Red, Green = Green + c.Green, Blue = Blue + c.Blue };
        public RenderColor Add(RenderColor c) { this = Plus(c); return this; }
        public RenderColor Minus(RenderColor c) => new() { Red = Math.Max(Red - c.Red, 0.0), Green = Math.Max(Green - c.Green, 0.0), Blue = Math.Max(Blue - c.Blue, 0.0) };
        public RenderColor Mult(double k) => new() { Red = Red * k, Green = Green * k, Blue = Blue * k };
        public RenderColor Mix(RenderColor c, double k) => new() { Red = Red * (1 - k) + c.Red * k, Green = Green * (1 - k) + c.Green * k, Blue = Blue * (1 - k) + c.Blue * k };
        public RenderColor Filter(RenderColor c) => new() { Red = Red * c.Red, Green = Green * c.Green, Blue = Blue * c.Blue };

        public bool Equals(RenderColor c)
        {
            const double eps = 1e-4;
            return Math.Abs(Red - c.Red) < eps && Math.Abs(Green - c.Green) < eps && Math.Abs(Blue - c.Blue) < eps;
        }

        public override string ToString() => string.Format(CultureInfo.InvariantCulture, "RGB({0:F3},{1:F3},{2:F3})", Red, Green, Blue);

        public static RenderColor operator +(RenderColor a, RenderColor b) => a.Plus(b);
        public static RenderColor operator -(RenderColor a, RenderColor b) => a.Minus(b);
        public static RenderColor operator *(RenderColor a, RenderColor b) => a.Filter(b);
        public static RenderColor operator *(RenderColor a, double k) => a.Mult(k);
        public static RenderColor operator *(double k, RenderColor a) => a.Mult(k);
    }
}
