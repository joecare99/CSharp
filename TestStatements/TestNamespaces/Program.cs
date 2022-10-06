using System;
using System.Reflection;

namespace TestNamespaces
{
    /// <summary>
    /// Class Program.
    /// </summary>
    class Program
    {
        static void Main1(string[] args)
        {
            Console.WriteLine($"Hello from {Assembly.GetExecutingAssembly().GetName()}");
            Console.ReadKey();
            {; ; }
        }
    }
}

namespace TestNamespaces2
{
    /// <summary>
    /// Class Program2.
    /// </summary>
    class Program2
    {
        static void Main(string[] args)
        {
            SubProc();

            {
                Console.WriteLine("Press any key ...");
            }

            void SubProc()
            {
                Console.WriteLine($"Hello from {Assembly.GetExecutingAssembly().GetName()}");
            }

            Console.ReadKey();
        }
    }
}
