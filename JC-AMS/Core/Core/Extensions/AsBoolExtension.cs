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
namespace JCAMS.Core.Extensions
{
    /// <summary>
    /// Class AsBoolExtension.
    /// </summary>
    public static class AsBoolExtension
    {

        /// <summary>
        /// Ases the bool.
        /// </summary>
        /// <param name="dr">The dr.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool AsBool(this object dr)
        {
            if (dr == null || dr.Equals(DBNull.Value)) { return false; }
            else if (dr is bool b) { return b; }
            else if (dr.AsString().ToLower().Contains("true")) { return true; }
            else if (dr.AsString().ToLower().Contains("false")) { return false; }
            else if (dr.AsInt32() == 1 || dr.AsInt32() == -1) { return true; }
            else return false;
        }
    }
}