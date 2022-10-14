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
using System.Text.RegularExpressions;
namespace JCAMS.Core.Extensions
{
    /// <summary>
    /// Class AsIntExtension.
    /// </summary>
    public static class SAsIntXtntn
    {

        /// <summary>
        /// Ases the int32.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>System.Int32.</returns>
        public static int AsInt32(string s)
        {
            s = s.Trim('\0', ' ');
            if (s.Length < 1)
            {
                return 0;
            }
            try
            {
                return int.Parse(s);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Ases the int32.
        /// </summary>
        /// <param name="dr">The dr.</param>
        /// <returns>System.Int32.</returns>
        public static int AsInt32(this object dr)
        {
            if (dr == null || dr.GetType() == typeof(object))
            {
                return 0;
            }
            if (Equals(dr, null) || dr.Equals(DBNull.Value) || dr.Equals("") || dr.Equals("\0\0"))
            {
                return 0;
            }
            try
            {
                return Convert.ToInt32(dr);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Ases the int64.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>System.Int64.</returns>
        public static long AsInt64(string s)
        {
            try
            {
                return long.Parse(s);
            }
            catch
            {
                return 0L;
            }
        }

        /// <summary>
        /// Ases the int64.
        /// </summary>
        /// <param name="dr">The dr.</param>
        /// <returns>System.Int64.</returns>
        public static long AsInt64(this object dr)
        {
            if (dr == null || dr.GetType() == typeof(object)) { return 0L; }
            if (Equals(dr, null) || dr.Equals(DBNull.Value) || dr.Equals("")) { return 0L; }
            try
            {
                return Convert.ToInt64(dr);
            }
            catch
            {
                return 0L;
            }
        }

        /// <summary>
		/// Determines whether the specified value is int.
		/// </summary>
		/// <param name="val">The value.</param>
		/// <returns><c>true</c> if the specified value is int; otherwise, <c>false</c>.</returns>
		public static bool IsInt(this string val)
        {
            if (string.IsNullOrEmpty(val)) return false;
            Regex r = new Regex("^(\\+|-)?\\d+$");
            return r.IsMatch(val);
        }

    }
}