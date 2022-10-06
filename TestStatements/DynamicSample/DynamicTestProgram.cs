using DynamicSample.Model;
using System;
using System.IO;

namespace DynamicSample
{
    /// <summary>
    /// Class DynamicTestProgram.
    /// </summary>

    public static class DynamicTestProgram
    {
        private static dynamic rFile;
        /// <summary>
        /// The s address
        /// </summary>
        private static DynPropertyClass sAddress;
        private static StatPropertyClass sAddress2=new StatPropertyClass();

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine(Constants.Constants.Header,"Example for dynamic class");
            ShowCustomer();
            ShowCustomerContains();
            ShowSupplier();
            ShowSupplierContains();
            Console.WriteLine(Constants.Constants.Header, "2. Example for dynamic class");
            CreateAddress();
            InitAddress(sAddress);
            ReadAddress(sAddress);
            ChangeAddress(sAddress);
            ReadAddress(sAddress);
            Console.WriteLine(Constants.Constants.Header, "2b. Example for static class");
            InitAddress(sAddress2);
            ReadAddress(sAddress2);
            ChangeAddress(sAddress2);
            ReadAddress(sAddress2);

            Console.ReadLine();
        }

        private static void InitAddress(dynamic sData)
        {
            sData.strName = "P. Mustermann";
            sData.strStreet = "Somestreet 35";
            sData.strCity = "Somplace";
            sData.strPLZ = "12345";
            sData.strCountry = "Somewhere";
            Console.WriteLine(sData.FullAddress);
            Console.WriteLine("----------------------------");
        }

        private static void ChangeAddress(dynamic sData)
        {
            sData.strName = "P. Musterfrau";
            sData.strStreet = "Someotherstreet 1";
            sData.strCity = "Anyplace";
            sData.strPLZ = "54321";
            sData.strCountry = "Nowhere";
            Console.WriteLine(sData.FullAddress);
            Console.WriteLine("----------------------------");
        }

        private static void ReadAddress(dynamic sData)
        {
            Console.WriteLine($"Name:\t{sData.strName}");
            Console.WriteLine($"Street:\t{sData.strStreet}");
            Console.WriteLine($"PLZ:\t{sData.strPLZ}");
            Console.WriteLine($"City:\t{sData.strCity}");
            Console.WriteLine($"Country:\t{sData.strCountry}");
            Console.WriteLine("----------------------------");
        }

        /// <summary>
        /// Shows the customer contains.
        /// </summary>
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

        /// <summary>
        /// Shows the customer.
        /// </summary>
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

        /// <summary>
        /// Shows the supplier contains.
        /// </summary>
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

        /// <summary>
        /// Shows the supplier.
        /// </summary>
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
            var DataDir = "TestStatements";
            for (var i = 0; i < 4; i++)
                if (Directory.Exists(DataDir+@"\DynamicSample")) break;
                else DataDir = ".." + @"\" + DataDir; 
            rFile = rFile ?? new ReadOnlyFile(DataDir+@"\DynamicSample\TextFile1.txt");
        }

        private static void CreateAddress()
        {
            sAddress = sAddress ?? new DynPropertyClass();
            foreach (string s in sAddress.GetDynamicMemberNames())
                Console.WriteLine(s);
            Console.WriteLine("----------------------------");
        }
    }
}
