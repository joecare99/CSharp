// ***********************************************************************
// Assembly         : JCAMS
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-24-2022
// ***********************************************************************
// <copyright file="T.cs" company="JC-Soft">
//     Copyright © JC-Soft 2008-2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Globalization;
namespace JCAMS.Core.Extensions
{
    /// <summary>
    /// Class AsDoubleExtension.
    /// </summary>
    public static class AsDoubleExtension
    {

        /// <summary>
        /// Ases the double.
        /// </summary>
        /// <param name="dr">The dr.</param>
        /// <returns>System.Double.</returns>
        public static double AsDouble(this object dr)
        {
            if (dr == null || dr.Equals(DBNull.Value))
            {
                return 0.0;
            }
            if (dr is double d) return d;
            if (dr is float f) return (double)f;
            if (dr is long l) return l;
            if (dr is byte[] b && b.Length>7) return BitConverter.ToDouble(b, 0);
            if (dr is int i) return i;
            if (dr is string s)
            {
                string Text = s.Trim();
                Text = Text.Replace(".", CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator);
                Text = Text.Replace(",", CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator);
                if (double.TryParse(Text, NumberStyles.Float, null, out double dd))
                {
                    return dd;
                }
            }
            return 0.0;
        }

        /// <summary>
        /// Ases the float.
        /// </summary>
        /// <param name="dr">The dr.</param>
        /// <returns>System.Single.</returns>
        public static float AsFloat(this object dr)
        {
            if (dr is byte[] b && b.Length > 3) return BitConverter.ToSingle(b,0);
            else
              return (float)dr.AsDouble();
        }
    }
}