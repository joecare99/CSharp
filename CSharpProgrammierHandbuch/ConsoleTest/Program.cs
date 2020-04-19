using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
    {
        static bool bUserBreak = false;
        static int Main(string[] args)
        {
            Console.CancelKeyPress += delegate { bUserBreak = true; Console.Write("<Userbreak>"); };
            Console.Title = " a custom Title";
            Console.WriteLine("Press some Keys ...");
            while (!bUserBreak)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo lKey = Console.ReadKey(true);
                    Console.WriteLine($"#{(int)lKey.KeyChar}: {lKey.KeyChar} ");
                }
                else
                {
                    Thread.Sleep(0);
                }
            }
            Console.WriteLine("... Programm beendet");
            Thread.Sleep(300);
//            Console.
            return 0;
        }
    }
}
