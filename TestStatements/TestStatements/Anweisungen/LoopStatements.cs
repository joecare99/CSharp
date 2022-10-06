// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 07-13-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="LoopStatements.cs" company="HP Inc.">
//     Copyright © HP Inc. 2020
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
    /// Class LoopStatements.
    /// </summary>
    public class LoopStatements
    {
        /// <summary>
        /// Does the while statement.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void DoWhileStatement(string[] args)
        {
            const string Title = "Beispiel für While-Schleife";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));

            int i = 0;
            while (i < args.Length)
            {
                Console.WriteLine(args[i]);
                i++;
            }
        }

        /// <summary>
        /// Does the do statement.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void DoDoStatement(string[] args)
        {
            const string Title = "Beispiel für Do-While Schleife";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));

            string s;
            do
            {
                s = Console.ReadLine() ?? "";
                Console.WriteLine(s);
            } while (!string.IsNullOrEmpty(s));
        }

        /// <summary>
        /// Does the do statement.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void DoDoStatement2(string[] args)
        {
            const string Title = "Beispiel für Do-While Schleife2";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));

            int i=0;
            do
                Console.WriteLine(i++.ToString());
            while (i < 10);
        }


        /// <summary>
        /// Does the do statement.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void DoDoStatement3(string[] args)
        {
            const string Title = "Beispiel für Do-While Schleife3";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));

            int i=0;
#pragma warning disable CS0642 // Möglicherweise falsche leere Anweisung
            do; // Warning but OK in syntax
#pragma warning restore CS0642 // Möglicherweise falsche leere Anweisung
            while (++i < 10);
        }

        /// <summary>
        /// Does the do statement.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void DoDoDoStatement(string[] args)
        {
            const string Title = "Beispiel für Do-While Schleife3";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));

            int i = 0;
#pragma warning disable CS0642 // Möglicherweise falsche leere Anweisung
            do do; // Warning but OK in syntax
#pragma warning restore CS0642 // Möglicherweise falsche leere Anweisung
                while (false);
            while (++i < 10);
        }

        /// <summary>
        /// Does the do statement.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void DoDoDoStatement2(string[] args)
        {
            const string Title = "Beispiel für Do-While Schleife3";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));

            int i = 0;
            do do { }
            while (false);
            while (++i < 10);
        }


        /// <summary>
        /// Does the do statement.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void DoDoWhileNestedStatement2(string[] args)
        {
            const string Title = "Beispiel für Do-While Schleife3";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));

            int i = 0;
            do
                i++; while (
false);
            while (++i < 10) ;
        }
        /// <summary>
        /// Does for statement.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void DoForStatement(string[] args)
        {
            const string Title = "Beispiel für For-Schleife";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));

            for (int i = 0; i < args.Length; i++)
            {
                Console.WriteLine(args[i]);
            }
        }

        /// <summary>
        /// Does for statement.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void DoForStatement2(string[] args)
        {
            const string Title = "Beispiel für For-Schleife";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));

            for (int i = 0; i < args.Length; i++)
            {
                Console.WriteLine(args[i]);
            }
        }

        /// <summary>
        /// Does for each statement.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void DoForEachStatement(string[] args)
        {
            const string Title = "Beispiel für Foreach-Schleife";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));

            foreach (string s in args)
            {
                Console.WriteLine(s);
            }
        }
    }
}
