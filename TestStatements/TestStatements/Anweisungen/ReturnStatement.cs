// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="ReturnStatement.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStatements.Anweisungen
{
    /// <summary>
    /// Class ReturnStatement.
    /// </summary>
    public class ReturnStatement
    {
        /// <summary>
        /// Adds the specified a.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>System.Int32.</returns>
        static int Add(int a, int b)
        {
            return a + b;
        }
        /// <summary>
        /// Does the return statement.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void DoReturnStatement(string[] args)
        {
            const string Title = "Beispiel für Return-Anweisung";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));

            Console.WriteLine(Add(1, 2));
            return;
        }
    }
}
