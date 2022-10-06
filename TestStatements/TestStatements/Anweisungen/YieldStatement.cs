using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStatements.Anweisungen
{
    public class YieldStatement
    {
        static System.Collections.Generic.IEnumerable<int> Range(int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                yield return i;
            }
            yield break;
        }
        public static void DoYieldStatement(string[] args)
        {
            const string Title = "Beispiel für Yield-Statement";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));

            foreach (int i in Range(-10, 10))
            {
                Console.WriteLine(i);
            }
        }

    }
}
