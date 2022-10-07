// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Checking.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace TestStatements.Anweisungen
{
    /// <summary>
    /// Class Checking.
    /// </summary>
    public class Checking
    {
        /// <summary>
        /// Do a checked and unchecked numeric calculation.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void CheckedUnchecked(string[] args)
        {
            int x = int.MaxValue;
            unchecked
            {
                Console.WriteLine(x + 1);  // Overflow
            }
            checked
            {
                try
                {
                    Console.WriteLine(x + 1);  // Exception
                }
                catch (OverflowException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
