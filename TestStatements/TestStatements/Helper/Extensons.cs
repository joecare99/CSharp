// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 09-20-2022
//
// Last Modified By : Mir
// Last Modified On : 09-20-2022
// ***********************************************************************
// <copyright file="Extensons.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Globalization;

namespace TestStatements.Helper
{
    /// <summary>
    /// Class Extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Tries zu parse the string as an int.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>System.Int32.</returns>
        public static int AsInt(this string s) => int.TryParse(s, out int i) ? i : 0;
        /// <summary>
        /// Tries zu parse the string as a float.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>System.Single.</returns>
        public static float AsFloat(this string s) => float.TryParse(s.Replace(",",CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator),NumberStyles.Any, CultureInfo.InvariantCulture, out float f) ? f : float.NaN;
        /// <summary>
        /// Tries zu parse the string as a double.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>System.Double.</returns>
        public static double AsDouble(this string s) => double.TryParse(s.Replace(",", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator), NumberStyles.Any, CultureInfo.InvariantCulture, out double d) ? d : double.NaN;

    }
}
