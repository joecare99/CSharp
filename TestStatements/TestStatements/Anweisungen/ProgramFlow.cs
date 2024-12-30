// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 07-13-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="ProgramFlow.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace TestStatements.Anweisungen
{
    /// <summary>
    /// Class ProgramFlow.
    /// </summary>
    public class ProgramFlow
    {
        /// <summary>
        /// Does the break statement.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void DoBreakStatement(string[] args)
        {
            const string Title = "Beispiel für Break-Anweisung";
            Console.WriteLine(Constants.Constants.Header, Title);

            while (true)
            {
                string s = Console.ReadLine() ?? "";
                if (string.IsNullOrEmpty(s))
                    break;
                Console.WriteLine(s);
            }
        }

        /// <summary>
        /// Does the continues statement.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void DoContinueStatement(string[] args)
        {
            const string Title = "Beispiel für Continue-Anweisung";
            Console.WriteLine(Constants.Constants.Header, Title);

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("/"))
                    continue;
                Console.WriteLine(args[i]);
            }
        }

        /// <summary>
        /// Does the goto statement.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void DoGoToStatement(string[] args)
        {
            const string Title = "Beispiel für Goto-Anweisung";
            Console.WriteLine(Constants.Constants.Header, Title);

            int i = 0;
            goto check;
        loop:
            Console.WriteLine(args[i++]);
        check:
            if (i < args.Length)
                goto loop;
        }
    }
}
