using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStatements.Anweisungen
{
    public class LoopStatements
    {
        /// <summary>Does the while statement.</summary>
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

        /// <summary>Does the do statement.</summary>
        /// <param name="args">The arguments.</param>
        public static void DoDoStatement(string[] args)
        {
            const string Title = "Beispiel für Do-While Schleife";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));

            string s;
            do
            {
                s = Console.ReadLine();
                Console.WriteLine(s);
            } while (!string.IsNullOrEmpty(s));
        }

        /// <summary>Does for statement.</summary>
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

        /// <summary>Does for each statement.</summary>
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
