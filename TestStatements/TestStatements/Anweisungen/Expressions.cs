using System;

namespace TestStatements.Anweisungen
{
    public class Expressions
    {
        /// <summary>Does expression statements.</summary>
        /// <param name="args">The arguments.</param>
        public static void DoExpressions(string[] args)
        {
            const string Title = "Beispiel für Ausdrücke";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));
            int i;
            i = 123;                // Expression statement
            Console.WriteLine(i);   // Expression statement
            i++;                    // Expression statement
            Console.WriteLine(i);   // Expression statement
        }
    }
}
