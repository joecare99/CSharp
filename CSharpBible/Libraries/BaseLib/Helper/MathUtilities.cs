using System;
using System.Linq;

namespace BaseLib.Helper
{
    public static class MathUtilities
    {
        public static double PT1(this double newvalue, ref double actvalue, double dTn_Ta = 0.1d, double KP = 1.0d) 
            => actvalue = dTn_Ta * KP * newvalue + (1d - dTn_Ta) * actvalue;

        public static double Mean(this double newvalue, double[] buffer, ref int ix)
        {
            buffer[ix++] = newvalue;
            ix = ix % buffer.Length;
            return buffer.Average(); 
        }
        public static double Median(this double newvalue, double[] buffer, ref int ix, double fMedian = 0.5d)
        {
            buffer[ix++] = newvalue;
            ix = ix % buffer.Length;
            var b = buffer.ToList<double>();
            b.Sort();
            return b[(int)Math.Floor(b.Count * fMedian)];
        }
    }
}
