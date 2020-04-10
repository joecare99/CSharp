using System;

namespace TestStatements.Anweisungen
{
    /// <summary>Example class for if statement</summary>
    public class ConditionalStatement
    {
        /// <summary>Does an if statement.</summary>
        /// <param name="args">The arguments.</param>
        public static void DoIfStatement(string[] args)
        {
            const string Title = "Auswertung von Bedingungen (IF-Anweisung)";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));
            if (args.Length == 0)
            {
                Console.WriteLine("No arguments");
            }
            else
            {
                Console.WriteLine("One or more arguments");
            }
        }

        public static void DoSwitchStatement(string[] args)
        {
            const string Title = "Auswertung von Bedingungen (SWITCH-Anweisung)";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));

            int n = args.Length;
            switch (n)
            {
                case 0:
                    Console.WriteLine("No arguments");
                    break;
                case 1:
                    Console.WriteLine("One argument");
                    break;
                default:
                    Console.WriteLine($"{n} arguments");
                    break;
            }
        }
    }
}
