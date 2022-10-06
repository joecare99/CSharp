using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestStatements.Constants;

namespace TestStatements.DataTypes
{
    public static class Formating
    {

        public static Func<DateTime> GetNow = delegate { return DateTime.Now; };

        /// <summary>Shows the combined formating.</summary>
        public static void CombinedFormating()
        {
            const string Title = "Combined Formating";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));

            string name = "Fred";
            Console.WriteLine(String.Format("Name = {0}, hours = {1:hh}", name, GetNow()));
        }

        /// <summary>Shows  the index-komponent.</summary>
        public static void IndexKomponent()
        {
            const string Title = "Formating with Index";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));

            string primes;
            primes = String.Format("Prime numbers less than 10: {0}, {1}, {2}, {3}",
                                   2, 3, 5, 7);
            Console.WriteLine(primes);
            // The example displays the following output:
            //      Prime numbers less than 10: 2, 3, 5, 7
        }

        public static void IndexKomponent2()
        {
            const string Title = "Formating with Index 2";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));

            string multiple = String.Format("0x{0:X} {0:E} {0:N}",
                                Int64.MaxValue);
            Console.WriteLine(multiple);
            // The example displays the following output:
            //      0x7FFFFFFFFFFFFFFF 9.223372E+018 9,223,372,036,854,775,807.00
        }

        public static void IndentationKomponent()
        {
            string[] names = { "Adam", "Bridgette", "Carla", "Daniel",
                         "Ebenezer", "Francine", "George" };
            decimal[] hours = { 40, 6.667m, 40.39m, 82, 40.333m, 80,
                                 16.75m };

            Console.WriteLine("{0,-20} {1,5}\n", "Name", "Hours");
            for (int ctr = 0; ctr < names.Length; ctr++)
                Console.WriteLine("{0,-20} {1,5:N1}", names[ctr], hours[ctr]);
        }

        public static void EscapeSequence()
        {
            int value = 6324;
            string output1 = string.Format("{{{0:D}}}", value);
            Console.WriteLine(output1);
            //       {D}        

            string output2 = string.Format("{0}{1:D}{2}",
                                         "{", value, "}");
            Console.WriteLine(output2);
            // The example displays the following output:
            //       {6324}        

            string output3 = string.Format("{{{0:D}{0:}}}", value);
            Console.WriteLine(output3);
            //       {D}        
        }

        public static void CodeExamples1()
        {
            string FormatString1 = String.Format("{0:dddd MMMM}", GetNow());
            Console.WriteLine(FormatString1);

            string FormatString2 = GetNow().ToString("dddd MMMM");
            Console.WriteLine(FormatString2);
        }

        public static void CodeExamples2()
        {
            int MyInt = 100;
            Console.WriteLine("{0:C}", MyInt);
            // The example displays the following output 
            // if en-US is the current culture:
            //        $100.00
        }

        public static void CodeExamples3()
        {
            string myName = "Fred";
            Console.WriteLine(String.Format("Name = {0}, hours = {1:hh}, minutes = {1:mm}",
                myName, GetNow()));
            // Depending on the current time, the example displays output like the following:
            //    Name = Fred, hours = 11, minutes = 30   
        }

        public static void CodeExamples4()
        {
            string myFName = "Fred";
            string myLName = "Opals";
            int myInt = 100;
            string FormatFName = String.Format("First Name = |{0,10}|", myFName);
            string FormatLName = String.Format("Last Name =  |{0,10}|", myLName);
            string FormatPrice = String.Format("Price =      |{0,10:C}|", myInt);
            Console.WriteLine(FormatFName);
            Console.WriteLine(FormatLName);
            Console.WriteLine(FormatPrice);
            Console.WriteLine();

            FormatFName = String.Format("First Name = |{0,-10}|", myFName);
            FormatLName = String.Format("Last Name =  |{0,-10}|", myLName);
            FormatPrice = String.Format("Price =      |{0,-10:C}|", myInt);
            Console.WriteLine(FormatFName);
            Console.WriteLine(FormatLName);
            Console.WriteLine(FormatPrice);
            // The example displays the following output on a system whose current
            // culture is en-US:
            //          First Name = |      Fred|
            //          Last Name = |     Opals|
            //          Price = |   $100.00|
            //
            //          First Name = |Fred      |
            //          Last Name = |Opals     |
            //          Price = |$100.00   |
        }
    }
}
