using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSample
{
    public static class DynamicTestProgram
    {
        private static dynamic rFile;
        public static void Main(string[] args)
        {
            Console.WriteLine(Constants.Constants.Header,"Example for dynamic class");
            ShowCustomer();
            ShowCustomerContains();
            ShowSupplier();
            ShowSupplierContains();
            Console.ReadLine();
        }

        public static void ShowCustomerContains()
        {
            Console.WriteLine(Constants.Constants.Header2, "Show Customers with Contains");
            CreateData();
            foreach (string line in rFile.Customer(StringSearchOption.Contains, true))
            {
                Console.WriteLine(line);
            }
            Console.WriteLine("----------------------------");
        }

        public static void ShowCustomer()
        {
            Console.WriteLine(Constants.Constants.Header2, "Show Customers");
            CreateData();
            foreach (string line in rFile.Customer)
            {
                Console.WriteLine(line);
            }
            Console.WriteLine("----------------------------");
        }

        public static void ShowSupplierContains()
        {
            Console.WriteLine(Constants.Constants.Header2, "Show Supplier with Contains");
            CreateData();
            foreach (string line in rFile.Supplier(StringSearchOption.Contains, true))
            {
                Console.WriteLine(line);
            }
            Console.WriteLine("----------------------------");
        }

        public static void ShowSupplier()
        {
            Console.WriteLine(Constants.Constants.Header2, "Show Suppliers");
            CreateData();
            foreach (string line in rFile.Supplier)
            {
                Console.WriteLine(line);
            }
            Console.WriteLine("----------------------------");
        }

        private static void CreateData()
        {
            rFile = rFile ?? new ReadOnlyFile(@"..\..\TestStatements\DynamicSample\TextFile1.txt");
        }
    }
}
