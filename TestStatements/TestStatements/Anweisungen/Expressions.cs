// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Expressions.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace TestStatements.Anweisungen
{
    /// <summary>
    /// Class Expressions.
    /// </summary>
    public class Expressions
    {
        /// <summary>
        /// Does expression statements.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void DoExpressions(string[] args)
        {
            const string Title = "Beispiel für Ausdrücke";
            Console.WriteLine(Constants.Constants.Header, Title);
            int i;
            i = 123;                // Expression statement
            Console.WriteLine(i);   // Expression statement
            i++;                    // Expression statement
            Console.WriteLine(i);   // Expression statement
        }
    }
}
