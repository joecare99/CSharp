using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStatements.Anweisungen
{
    public class ReturnStatement
    {
        static int Add(int a, int b)
        {
            return a + b;
        }
        /// <summary>Does the return statement.
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
