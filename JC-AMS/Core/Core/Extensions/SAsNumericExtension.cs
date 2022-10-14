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
using JCAMS.Core.Logging;
using System;
using System.Drawing;
using System.Globalization;
namespace JCAMS.Core.Extensions
{
    /// <summary>
    /// Class AsNumericExtension.
    /// </summary>
    public static class SAsNumericExtension
    {

        /// <summary>
        /// Ases the date time.
        /// </summary>
        /// <param name="dr">The dr.</param>
        /// <returns>DateTime.</returns>
        public static DateTime AsDateTime(this object dr)
        {
            DateTime dt = DateTime.MinValue;
            if (dr == null || dr.Equals(DBNull.Value))
            {
                return dt;
            }
            try
            {
                if (dr.GetType() == typeof(DateTime))
                {
                    dt = (DateTime)dr;
                }
                else if (dr.GetType() == typeof(string))
                {
                    dt = default;
                    DateTime.TryParse(dr.ToString(), out dt);
                }
                else
                {
                    dt = (DateTime)dr;
                }
            }
            catch (Exception Ex)
            {
                SLogging.Log(Ex);
            }
            return dt;
        }

        /// <summary>
        /// Ases the size.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Size.</returns>
        public static Size AsSize(this object obj)
        {
            if (obj == null) return Size.Empty;
            else if (obj is int[] ai && ai.Length>=2) return new Size(ai[0], ai[1]);
            string[] aS = Convert.ToString(obj).Split(';');
            if (aS.Length == 2)
            {
                return new Size(aS[0].AsInt32(), aS[1].AsInt32());
            }
            return Size.Empty;
        }

        /// <summary>
        /// Ases the string.
        /// </summary>
        /// <param name="dr">The dr.</param>
        /// <returns>System.String.</returns>
        public static string AsString(this object dr)
        {
            if (dr == null || dr.Equals(DBNull.Value)) return ""; 
            else if (dr is string s) return s;
            else return dr.ToString();
        }


        /// <summary>
        /// Ases the timestamp.
        /// </summary>
        /// <param name="dr">The dr.</param>
        /// <returns>System.String.</returns>
        public static string AsTimeStampS(this object dr)
        {
            if (dr == null) return $"0x{0:X16}";
            else if (long.TryParse(dr.ToString().Trim(), NumberStyles.Number, null, out long I))
                  return $"0x{I:X16}";
            return $"0x{dr.AsTimeStampL():X16}";

        }

        /// <summary>
        /// Ases the time stamp.
        /// </summary>
        /// <param name="dr">The dr.</param>
        /// <returns>System.Int64.</returns>
        public static long AsTimeStampL(this object dr)
        {
            if (dr == null) return 0L;
            else if (dr is string s && s.StartsWith("0x")) return Convert.ToInt64(s.Substring(2),16);
            else if (dr is double d) return (long)d;
            else if (dr is TimeSpan ts) return (long)ts.TotalMilliseconds;
            else if (dr is long l) return l;
            else if (dr is int i) return (long)i;
            else if (dr is byte[] arrB)
            {
                long Result = 0L;
                for (var J = 0; J < arrB.Length; J++)
                {
                    Result = (Result << 8) + arrB[J];
                }
                return Result;
            }
            else return 0L;
        }

        /// <summary>
        /// Ases the u int64.
        /// </summary>
        /// <param name="dr">The dr.</param>
        /// <returns>System.UInt64.</returns>
        public static ulong AsUInt64(this object dr)
        {

            if (dr == null|| dr.Equals(DBNull.Value) || dr.Equals("") || dr.GetType()==typeof(object)) return 0uL;            
            else if (dr is string s && s.StartsWith("0x")) return Convert.ToUInt64(s.Substring(2), 16);
            else if (dr is double d) return (ulong)d;
            else if (dr is int i) return (ulong)i;
            else if (dr is byte[] arrB)
            {
                ulong Result = 0uL;
                for (var J = 0; J < arrB.Length; J++)
                {
                    Result = (Result << 8) + arrB[J];
                }
                return Result;
            }
            else try
            {
                return Convert.ToUInt64(dr);
            }
            catch
            {
                return 0uL;
            }
        }
    }
}