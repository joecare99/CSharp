// ***********************************************************************
// Assembly         : JCAMS
// Author           : Mir
// Created          : 09-24-2022
//
// Last Modified By : Mir
// Last Modified On : 09-24-2022
// ***********************************************************************
// <copyright file="TAssertions.cs" company="JC-Soft">
//     Copyright © JC-Soft 2008-2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Diagnostics;
// ***********************************************************************
// Assembly         : JCAMS
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 07-11-2020
// ***********************************************************************
// <copyright file="T.cs" company="JC-Soft">
//     Copyright © JC-Soft 2008-2015
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace JCAMS.Core
{
    /// <summary>
    /// Class TAssertions.
    /// </summary>
    public static class SAssertions
    {

        /// <summary>
        /// Asserts the specified condition.
        /// </summary>
        /// <param name="Condition">if set to <c>true</c> [condition].</param>
        public static void Assert(bool Condition)
        {
            Debug.Assert(Condition);
        }

        /// <summary>
        /// Asserts the specified condition.
        /// </summary>
        /// <param name="Condition">if set to <c>true</c> [condition].</param>
        /// <param name="Message">The message.</param>
        public static void Assert(bool Condition, string Message)
        {
            Debug.Assert(Condition, Message);
        }

        /// <summary>
        /// Asserts the specified condition.
        /// </summary>
        /// <param name="Condition">if set to <c>true</c> [condition].</param>
        /// <param name="Message">The message.</param>
        /// <param name="DetailMessage">The detail message.</param>
        public static void Assert(bool Condition, string Message, string DetailMessage)
        {
            Debug.Assert(Condition, Message, DetailMessage);
        }
    }
}