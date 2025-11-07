using System;

namespace RenderImage.Base.Model
{
    public static class RenderMath
    {
        public static bool SolveQuadratic(double a, double b, double c, out double l1, out double l2)
        {
            // Portiert aus unt_RenderMath.pas
            if (a == 0 && b == 0 && c == 0)
            {
                l1 = 1; l2 = -1; return true;
            }
            if (a == 0 && b == 0 && c != 0)
            {
                l1 = 0; l2 = 0; return false;
            }
            if (a == 0 && b != 0)
            {
                l1 = -c / b; l2 = l1; return true;
            }
            if (b == 0 && (-c / a) < 0)
            {
                l1 = 0; l2 = 0; return false;
            }
            if (b == 0 && (-c / a) >= 0)
            {
                var t = Math.Sqrt(-c / a);
                l1 = -t; l2 = t; return true;
            }
            if (c == 0)
            {
                if (b * a > 0)
                { l2 = 0; l1 = -b / a; }
                else
                { l1 = 0; l2 = -b / a; }
                return true;
            }
            var disc = b * b - 4 * a * c;
            var ok = disc >= 0;
            if (ok)
            {
                var s = Math.Sqrt(disc);
                l1 = (-b - s) / (2 * a);
                l2 = (-b + s) / (2 * a);
                return true;
            }
            l1 = 0; l2 = 0; return false;
        }
    }
}
