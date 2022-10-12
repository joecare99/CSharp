// ***********************************************************************
// Assembly         : JCAMS
// Author           : Mir
// Created          : 09-24-2022
//
// Last Modified By : Mir
// Last Modified On : 09-24-2022
// ***********************************************************************
// <copyright file="ListExtensions.cs" company="JC-Soft">
//     Copyright © JC-Soft 2008-2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using JCAMS.Core.Logging;
using System;
using System.Collections.Generic;

namespace JCAMS.Core.Extensions
{
    /// <summary>
    /// Class ListExtensions.
    /// </summary>
    public static class ListExtensions
    {

        /// <summary>
        /// Determines whether the specified list contains the string [case insensitive].
        /// </summary>
        /// <param name="list">The [string]-list.</param>
        /// <param name="aString">The string.</param>
        /// <returns><c>true</c> if the specified list contains the string (case insensitive); otherwise, <c>false</c>.</returns>
        public static bool ContainsCI(this List<string> list, string aString)
        {
            try
            {
                if (string.IsNullOrEmpty(aString) || list == null) { return false; }
                aString = aString.ToLower();
                foreach (var sEnt in list)
                {
                    var sEntLwr = sEnt.ToLower();
                    if (sEntLwr == aString
                        || sEntLwr.StartsWith(aString + " ")
                        || sEntLwr.Contains(" " + aString + " "))
                        return true; 
                }
            }
            catch (Exception Ex)
            {
                TLogging.Log(Ex);
            }
            return false;
        }
    }
}