// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Declarations.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace TestStatements.Anweisungen
{
    /// <summary>
    /// Class Declarations.
    /// </summary>
    public class Declarations
    {
        /// <summary>
        /// Does variable declarations.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void DoVarDeclarations(string[] args)
        {
            const string Title = "Deklaration von Variablen";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));
            int a;
            int b = 2, c = 3;
            a = 1;
            Console.WriteLine(a + b + c);
        }

        /// <summary>
        /// Does constant declarations.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void DoConstantDeclarations(string[] args)
        {
            const string Title = "Deklaration von Konstanten";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));
            const float pi = 3.1415927f;
            const int r = 25;
            Console.WriteLine(pi * r * r);
        }
    }
}
