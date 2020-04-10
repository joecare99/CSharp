using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStatements.Anweisungen
{
    public class Checking
    {
        public static void CheckedUnchecked(string[] args)
        {
            int x = int.MaxValue;
            unchecked
            {
                Console.WriteLine(x + 1);  // Overflow
            }
            checked
            {
                try
                {
                    Console.WriteLine(x + 1);  // Exception
                }
                catch (OverflowException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
