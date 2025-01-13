// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 09-20-2022
//
// Last Modified By : Mir
// Last Modified On : 09-20-2022
// ***********************************************************************
// <copyright file="ExtensionExample.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace TestStatements.Helper
{
    /// <summary>
    /// Class ExtensionExample.
    /// </summary>
    public static class ExtensionExample
    {
        /// <summary>
        /// Shows the extension ex1.
        /// </summary>
        public static void ShowExtensionEx1()
        {
            const string Title = "Example for Extension (Helper)";
            Console.WriteLine(Constants.Constants.Header, Title);
            Console.WriteLine();
            Console.WriteLine("123".AsInt() + 5);
            Console.WriteLine("12,3".AsFloat() -1.0);
            Console.WriteLine("12.3".AsFloat() + 1.0);
            Console.WriteLine("12.3".AsDouble() + 1.0);
            Console.WriteLine("12,3e2".AsDouble() + 1.0);
        }
    }
}
