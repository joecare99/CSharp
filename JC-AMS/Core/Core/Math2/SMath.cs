// ***********************************************************************
// Assembly         : JCAMS
// Author           : Mir
// Created          : 09-22-2022
//
// Last Modified By : Mir
// Last Modified On : 09-24-2022
// ***********************************************************************
// <copyright file="CMath.cs" company="JC-Soft">
//     Copyright © JC-Soft 2008-2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using JCAMS.Core.Logging;

namespace JCAMS.Core.Math2
{
    /// <summary>
    /// Class CMath.
    /// </summary>
    public static class SMath
    {
        /// <summary>
        /// The wurzel aus 2
        /// </summary>
        public static readonly double SqRoot_Of_2 = Math.Sqrt(2d);

        /// <summary>
        /// The pi div 180
        /// </summary>
        public static readonly double PI_DIV_180 = Math.PI / 180d;

        /// <summary>
        /// Arithmetics the average.
        /// </summary>
        /// <param name="M">The m.</param>
        /// <param name="X">The x.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool ArithmeticAverage(out double M, IEnumerable<double> X)
        {
            M = double.NaN;
            if (X == null || X.Count() < 1)  return false;
            M = X.ArithmeticAverage();
            return true;
        }

        /// <summary>
        /// Arithmetics the average.
        /// </summary>
        /// <param name="M">The m.</param>
        /// <param name="adValues">The array of values.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool ArithmeticAverage(out double M, double[] adValues) 
            => ArithmeticAverage(out M, adValues?.ToList());

        /// <summary>
        /// Arithmetics the average.
        /// </summary>
        /// <param name="M">The m.</param>
        /// <param name="SyncQueue">The synchronize queue.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool ArithmeticAverage(out double M, Queue<double> SyncQueue)
        {
            if (SyncQueue==null || SyncQueue.Count<1 )
            {
                M = double.NaN;
                return false;
            }
            try
            {
                M = SyncQueue.ToArray().ArithmeticAverage();
                return true; 
            }
            catch (Exception ex)
            {
                SLogging.Log(ex);
                M = double.NaN;
                return false;
            }
        }

        /// <summary>
        /// Calculates the arithmetic average.
        /// </summary>
        /// <param name="X">The Field of values.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static double ArithmeticAverage(this IEnumerable<double> X) 
            => X == null || X.Count() < 1 ? double.NaN : X.Average();

        /// <summary>
        /// Calculates the arithmetic average.
        /// </summary>
        /// <param name="X">The Field of values.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static double ArithmeticAverage(this double[] X)
            => X == null || X.Count() < 1 ? double.NaN : X.Average();

        /// <summary>
        /// Calculate the process-value Cp from dMin and dMax
        /// </summary>
        /// <param name="dCp">The d cp.</param>
        /// <param name="dMin">The d minimum.</param>
        /// <param name="dMax">The d maximum.</param>
        /// <param name="dSigma">The d sigma.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool Cp(out double dCp, double dMin, double dMax, double dSigma)
        {
            dCp = double.NaN;
            if (dMax == dMin) { return false; } //??
            if (dSigma == 0.0) { return false; } 
            dCp = Math.Abs(dMax - dMin) / (6.0 * dSigma);
            return true;
        }

        /// <summary>
        /// Calculate the process-value Cp from dMin and dMax
        /// </summary>
        /// <param name="dCp">The d cp.</param>
        /// <param name="dMin">The d minimum.</param>
        /// <param name="dMax">The d maximum.</param>
        /// <param name="dSigma">The d sigma.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static double Cp(double dMin, double dMax, double dSigma)
        {
            if (dMax == dMin) { return double.NaN; } //??
            if (dSigma == 0.0) { return double.NaN; } 
            return Math.Abs(dMax - dMin) / (6.0 * dSigma);
        }

        /// <summary>
        /// Cps the k.
        /// </summary>
        /// <param name="dCpk">The d CPK.</param>
        /// <param name="dMin">The d minimum.</param>
        /// <param name="dMax">The d maximum.</param>
        /// <param name="dMW">The d mw.</param>
        /// <param name="dSigma">The d sigma.</param>
        /// <param name="iNGFlag">The i ng flag.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool CpK(out double dCpk, double dMin, double dMax, double dMW, double dSigma, bool xNGFlag)
        {
            dCpk = double.NaN;
            if (dSigma == 0.0) { return false; }
            if (!double.IsNaN(dMax) && !double.IsNaN(dMin))
            {
                if (dMax == dMin) { return false; }
                if ((dMax == 0.0 || dMin == 0.0) && !xNGFlag )
                {
                    if (dMax == 0.0)
                    {
                        dCpk = (dMW - dMin) / (3.0 * dSigma);
                    }
                    else
                    {
                        dCpk = (dMax - dMW) / (3.0 * dSigma);
                    }
                }
                else
                {
                    double dCpw = (dMax - dMin) / (6.0 * dSigma);
                    double dMW2 = (dMax + dMin) / 2.0;
                    double dMW3 = 0.5 * (dMax - dMin);
                    double dKWert = Math.Abs((dMW2 - dMW) / dMW3);
                    dCpk = dCpw * (1.0 - dKWert);
                }
            }
            else
            {
                if (xNGFlag) { return false; }
                if (double.IsNaN(dMax))
                {
                    if (dMin == 0.0)
                    {
                        return false;
                    }
                    dCpk = (dMW - dMin) / (3.0 * dSigma);
                }
                else
                {
                    if (dMax == 0.0) { return false; }
                    dCpk = (dMax - dMW) / (3.0 * dSigma);
                }
            }
            return true;
        }

        /// <summary>
        /// Deg2s the RAD.
        /// </summary>
        /// <param name="Deg">The deg.</param>
        /// <returns>System.Single.</returns>
        public static float Deg2Rad(float Deg)
        {
            return (float)((double)Deg * PI_DIV_180);
        }

        /// <summary>
        /// Deg2s the RAD.
        /// </summary>
        /// <param name="Deg">The deg.</param>
        /// <returns>System.Double.</returns>
        public static double Deg2Rad(double Deg)
        {
            return Deg * PI_DIV_180;
        }


        /// <summary>
        /// Limits the Value between the Minimum and the maximum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Lower">The lower.</param>
        /// <param name="Value">The value.</param>
        /// <param name="Upper">The upper.</param>
        /// <returns>System.Double.</returns>
        public static T MinMax<T>(T Lower, T Value, T Upper) 
            => Value.Limit(Lower, Upper);

        /// <summary>
        /// Limits the Value between the Minimum and the maximum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Lower">The lower.</param>
        /// <param name="Value">The value.</param>
        /// <param name="Upper">The upper.</param>
        /// <returns>System.Double.</returns>
        public static T Limit<T>(this T Value, T Lower, T Upper)
        {
            if (Comparer<T>.Default.Compare(Value, Upper) > 0)
            {
                return Upper;
            }
            if (Comparer<T>.Default.Compare(Lower, Value) > 0)
            {
                return Lower;
            }
            return Value;
        }

        /// <summary>
        /// Deltas the angle.
        /// </summary>
        /// <param name="Angle1">The angle1.</param>
        /// <param name="Angle2">The angle2.</param>
        /// <returns>System.Double.</returns>
        public static double DeltaAngle(double Angle1, double Angle2)
        {
            return (Angle2-Angle1).NormalizeAngle(0d);
        }

        /// <summary>
        /// Deltas the angle clockwise.
        /// </summary>
        /// <param name="Angle1">The angle1.</param>
        /// <param name="Angle2">The angle2.</param>
        /// <returns>System.Double.</returns>
        public static double DeltaAngleClockwise(double Angle1, double Angle2)
        {
            return (Angle2 - Angle1).NormalizeAngle();
        }

        /// <summary>
        /// Determines whether the specified value is between.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value">The value.</param>
        /// <param name="Lower">The lower.</param>
        /// <param name="Upper">The upper.</param>
        /// <returns><c>true</c> if the specified value is between; otherwise, <c>false</c>.</returns>
        public static bool IsBetween<T>(this T Value, T Lower, T Upper)
        {
            if (Comparer<T>.Default.Compare(Lower, Upper) < 0)
            {
                return Comparer<T>.Default.Compare(Lower, Value) <= 0 && Comparer<T>.Default.Compare(Value, Upper) <= 0;
            }
            return Comparer<T>.Default.Compare(Lower, Value) >= 0 && Comparer<T>.Default.Compare(Value, Upper) >= 0;
        }


        /// <summary>
        /// Determines whether the specified value is between.
        /// </summary>
        /// <param name="Value">The value.</param>
        /// <param name="Lower">The lower.</param>
        /// <param name="Upper">The upper.</param>
        /// <returns><c>true</c> if the specified value is between; otherwise, <c>false</c>.</returns>
        public static bool IsBetween(double Value, double Lower, double Upper) =>
            Value.IsBetween(Lower, Upper);

        /// <summary>
        /// Determines whether the specified value is between.
        /// </summary>
        /// <param name="Value">The value.</param>
        /// <param name="Lower">The lower.</param>
        /// <param name="Upper">The upper.</param>
        /// <returns><c>true</c> if the specified value is between; otherwise, <c>false</c>.</returns>
        public static bool IsBetween(int Value, int Lower, int Upper) =>
            Value.IsBetween(Lower, Upper);

        /// <summary>
        /// Normalizes the angle.
        /// </summary>
        /// <param name="Angle">The angle.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool NormalizeAngle(ref int Angle)
        {
            Angle = (Angle % 360 + 360) % 360;
            return true;
        }

        /// <summary>
        /// Normalizes the angle.
        /// </summary>
        /// <param name="Angle">The angle.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool NormalizeAngle(ref float Angle)
        {
            Angle = (float)((double)Angle).NormalizeAngle();
            return true;
        }

        /// <summary>
        /// Normalizes the angle.
        /// </summary>
        /// <param name="Angle">The angle.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool NormalizeAngle(ref double Angle)
        {
            Angle = Angle.NormalizeAngle();
            return true;
        }

        /// <summary>
        /// Normalizes the angle.
        /// </summary>
        /// <param name="Angle">The angle.</param>
        /// <returns>System.Double.</returns>
        public static double NormalizeAngle(this double Angle,double dMid = 180d)
        {
            return Angle -Math.Floor((Angle-dMid+180d)/360d)*360d;
        }

        /// <summary>
        /// Rad2s the deg.
        /// </summary>
        /// <param name="Rad">The RAD.</param>
        /// <returns>System.Single.</returns>
        public static float Rad2Deg(float Rad)
        {
            return (float)((double)Rad / PI_DIV_180);
        }

        /// <summary>
        /// Rad2s the deg.
        /// </summary>
        /// <param name="Rad">The RAD.</param>
        /// <returns>System.Double.</returns>
        public static double Rad2Deg(double Rad)
        {
            return Rad / PI_DIV_180;
        }

        /// <summary>
        /// Computes the standards-deviation (not. corr.)
        /// </summary>
        /// <param name="S">The s.</param>
        /// <param name="X">The x.</param>
        /// <param name="Predictand">The predictand.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool StandardDeviation(out double S, double[] X, double Predictand) =>
            StandardDeviation(out S, (X == null) ? null : X.ToList(), Predictand);

        /// <summary>
        /// Computes the standards-deviation (not. corr.)
        /// </summary>
        /// <param name="S">The s.</param>
        /// <param name="X">The x.</param>
        /// <param name="Predictand">The predictand.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool StandardDeviation(out double S, IEnumerable<double> X, double Predictand)
        {
            S = double.NaN;
            if (X==null || Predictand.Equals(double.NaN))
            {
                return false;
            }
            double dSumme = 0.0;
            double dCount = 0.0;
            foreach (var d in X)
            { dSumme += SMath.Sqr(d - Predictand); dCount++; }
            if (dCount >= 0.9)
            S = Math.Sqrt(dSumme / dCount);
            // for the corrected Value the Sum has to be divided by (dCount-1)
            return true;
        }

        /// <summary>
        /// Computes the standards-deviation (not. corr.)
        /// </summary>
        /// <param name="S">The s.</param>
        /// <param name="X">The x.</param>
        /// <param name="Predictand">The predictand.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static double StandardDeviation(this IEnumerable<double> X, double Predictand=double.NaN)
        {
            if (X == null || X.Count() < 1) return double.NaN;
            if (double.IsNaN(Predictand)) Predictand = X.Average();
            return Math.Sqrt(X.Average((d) => SMath.Sqr(d - Predictand)));
            // for the corrected Value the Sum has to be divided by (dCount-1)        
        }

        /// <summary>
        /// Computes the Square of the Value
        /// </summary>
        /// <param name="dValue">The value</param>
        public static double Sqr(this double dValue) => dValue*dValue;

        /// <summary>
        /// Trims the start- or stopangle by 180. (??)
        /// </summary>
        /// <param name="AngleStart">The angle start.</param>
        /// <param name="AngleStop">The angle stop.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool TrimAngle180(ref double AngleStart, ref double AngleStop)
        {
            if (Math.Abs(AngleStart - AngleStop) > 90.0)
            {
                if (AngleStart - AngleStop > 90.0)
                {
                    AngleStop += 180.0;
                }
                else
                {
                    AngleStart += 180.0;
                }
            }
            return true;
        }

        /// <summary>
        /// Trims the start- or stopangle by 360. (??)
        /// </summary>
        /// <param name="AngleStart">The angle start.</param>
        /// <param name="AngleStop">The angle stop.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool TrimAngle360(ref double AngleStart, ref double AngleStop)
        {
            if (Math.Abs(AngleStart - AngleStop) > 180.0)
            {
                if (AngleStart - AngleStop > 180.0)
                {
                    AngleStop += 360.0;
                }
                else
                {
                    AngleStart += 360.0;
                }
            }
            return true;
        }
    }
}
