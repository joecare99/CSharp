using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStatements.Anweisungen
{
    public class UsingStatement
    {
        public static void DoUsingStatement(string[] args)
        {
            using (TextWriter w = File.CreateText("test.txt"))
            {
                w.WriteLine("Line one");
                w.WriteLine("Line two");
                w.WriteLine("Line three");
            }
            using (TextReader r = File.OpenText("test.txt"))
            {
                const string Title = "Beispiel für Using-Statement";
                Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));

                Console.WriteLine(r.ReadLine());
                Console.WriteLine(r.ReadLine());
                Console.WriteLine(r.ReadLine());                
            }
        }
    }
}