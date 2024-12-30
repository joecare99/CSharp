// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="YieldStatement.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace TestStatements.Anweisungen
{
    /// <summary>
    /// Class YieldStatement.
    /// </summary>
    public class YieldStatement
    {
        /// <summary>
        /// Ranges the specified start.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>System.Collections.Generic.IEnumerable&lt;System.Int32&gt;.</returns>
        static System.Collections.Generic.IEnumerable<int> Range(int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                yield return i;
            }
            yield break;
        }
        /// <summary>
        /// Does the yield statement.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void DoYieldStatement(string[] args)
        {
            const string Title = "Beispiel für Yield-Statement";
            Console.WriteLine(Constants.Constants.Header, Title);

            foreach (int i in Range(-10, 10))
            {
                Console.WriteLine(i);
            }
        }

    }
}
