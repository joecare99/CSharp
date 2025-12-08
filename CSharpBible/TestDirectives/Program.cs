// ***********************************************************************
// Assembly         : TestDirectives
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 10-04-2020
// ***********************************************************************
// <copyright file="Program.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
#define Test
#define MyDef 
using System;

namespace TestDirectives
{
    /// <summary>
    /// Class Program.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            Console.WriteLine("Normal line #1."); // Set break point here.
#line hidden
            Console.WriteLine("Hidden line.");
#line default
            Console.WriteLine("Normal line #2.");
        }
    }
}
