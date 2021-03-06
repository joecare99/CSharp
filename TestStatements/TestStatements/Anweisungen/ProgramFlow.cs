﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStatements.Anweisungen
{
    public class ProgramFlow
    {
        /// <summary>Does the break statement.</summary>
        /// <param name="args">The arguments.</param>
        public static void DoBreakStatement(string[] args)
        {
            const string Title = "Beispiel für Break-Anweisung";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));

            while (true)
            {
                string s = Console.ReadLine();
                if (string.IsNullOrEmpty(s))
                    break;
                Console.WriteLine(s);
            }
        }

        /// <summary>Does the continues statement.</summary>
        /// <param name="args">The arguments.</param>
        public static void DoContinueStatement(string[] args)
        {
            const string Title = "Beispiel für Continue-Anweisung";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("/"))
                    continue;
                Console.WriteLine(args[i]);
            }
        }

        /// <summary>Does the goto statement.</summary>
        /// <param name="args">The arguments.</param>
        public static void DoGoToStatement(string[] args)
        {
            const string Title = "Beispiel für Goto-Anweisung";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));

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
