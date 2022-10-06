// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-10-2022
// ***********************************************************************
// <copyright file="RandomExample.cs" company="HP Inc.">
//     Copyright © HP Inc. 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStatements.Anweisungen
{
    /// <summary>
    /// Class RandomExample.
    /// </summary>
    public static class RandomExample
    {

        /// <summary>
        /// Instantiate random number generator using system-supplied value as seed.
        /// The rand
        /// </summary>
        public static Random rand = new Random();

        /// <summary>
        /// Examples the main1.
        /// </summary>
        /// <remarks>The example displays output like the following:<br />
        /// Five random byte values:<br />
        /// 194  185  239   54  116<br />
        /// Five random integer values:<br />
        /// 507,353,531  1,509,532,693  2,125,074,958  1,409,512,757    652,767,128<br />
        /// Five random integers between 0 and 100:<br />
        /// 16      78      94      79      52<br />
        /// Five random integers between 50 and 100:<br />
        /// 56      66      96      60      65<br />
        /// Five Doubles.<br />
        /// 0.943   0.108   0.744   0.563   0.415<br />
        /// Five Doubles between 0 and 5.<br />
        /// 2.934   3.130   0.292   1.432   4.369</remarks>
        public static void ExampleMain1()
        {
            GenerateFiveRandomBytes();
            GenerateFiveRandomIntegers();
            GenerateFiveRandomIntegersRanged();
            GenerateFiveRandomIntegersRanged2();
            GenerateFiveFloatingPointValues();
            GenerateFiveFloatingPointValuesRanged();
        }

        /// <summary>
        /// Generate and display 5 random floating point values from 0 to 5.
        /// </summary>
        private static void GenerateFiveFloatingPointValuesRanged()
        {
            // Generate and display 5 random floating point values from 0 to 5.
            Console.WriteLine("Five Doubles between 0 and 5.");
            for (int ctr = 0; ctr <= 4; ctr++)
                Console.Write("{0,8:N3}", rand.NextDouble() * 5);
        }

        /// <summary>
        /// Generates the five floating point values.
        /// </summary>
        private static void GenerateFiveFloatingPointValues()
        {
            // Generate and display 5 random floating point values from 0 to 1.
            Console.WriteLine("Five Doubles.");
            for (int ctr = 0; ctr <= 4; ctr++)
                Console.Write("{0,8:N3}", rand.NextDouble());
            Console.WriteLine();
        }

        /// <summary>
        /// Generates the five random integers ranged2.
        /// </summary>
        private static void GenerateFiveRandomIntegersRanged2()
        {
            // Generate and display 5 random integers from 50 to 100.
            Console.WriteLine("Five random integers between 50 and 100:");
            for (int ctr = 0; ctr <= 4; ctr++)
                Console.Write("{0,8:N0}", rand.Next(50, 101));
            Console.WriteLine();
        }

        /// <summary>
        /// Generates the five random integers ranged.
        /// </summary>
        private static void GenerateFiveRandomIntegersRanged()
        {
            // Generate and display 5 random integers between 0 and 100.
            Console.WriteLine("Five random integers between 0 and 100:");
            for (int ctr = 0; ctr <= 4; ctr++)
                Console.Write("{0,8:N0}", rand.Next(101));
            Console.WriteLine();
        }

        /// <summary>
        /// Generates (and display) the five random integers.
        /// </summary>
        /// <remarks>Generate and display 5 random integers.</remarks>
        private static void GenerateFiveRandomIntegers()
        {
            Console.WriteLine("Five random integer values:");
            for (int ctr = 0; ctr <= 4; ctr++)
                Console.Write("{0,15:N0}", rand.Next());
            Console.WriteLine();
        }

        /// <summary>
        /// Generate and display 5 random byte (integer) values.
        /// </summary>
        /// <remarks>Generate and display 5 random byte (integer) values.</remarks>
        private static void GenerateFiveRandomBytes()
        {
            var bytes = new byte[5];
            rand.NextBytes(bytes);
            Console.WriteLine("Five random byte values:");
            foreach (byte byteValue in bytes)
                Console.Write("{0, 5}", byteValue);
            Console.WriteLine();
        }

        /// <summary>
        /// Examples the main2.
        /// </summary>
        /// <remarks>The example displays the following output e.G:<br />
        /// Suggested pet name of the day:<br />
        /// For a male:     Koani<br />
        /// For a female:   Maggie</remarks>
        public static void ExampleMain2()
        {
            string[] malePetNames = { "Rufus", "Bear", "Dakota", "Fido",
                          "Vanya", "Samuel", "Koani", "Volodya",
                          "Prince", "Yiska" };
            string[] femalePetNames = { "Maggie", "Penny", "Saya", "Princess",
                            "Abby", "Laila", "Sadie", "Olivia",
                            "Starlight", "Talla" };

            // Generate random indexes for pet names.
            int mIndex = rand.Next(malePetNames.Length);
            int fIndex = rand.Next(femalePetNames.Length);

            // Display the result.
            Console.WriteLine("Suggested pet name of the day: ");
            Console.WriteLine("   For a male:     {0}", malePetNames[mIndex]);
            Console.WriteLine("   For a female:   {0}", femalePetNames[fIndex]);

            // The example displays the following output:
            //       Suggested pet name of the day:
            //          For a male:     Koani
            //          For a female:   Maggie
        }

        /// <summary>
        /// Examples the main3.
        /// </summary>
        /// <remarks>The example displays output like the following:<br />
        /// First Series:<br />
        /// 97  129  149   54   22  208  120  105   68  177<br />
        /// 113  214   30  172   74  218  116  230   89   18<br />
        /// 12  112  130  105  116  180  190  200  187  120<br />
        /// 7  198  233  158   58   51   50  170   98   23<br />
        /// 21    1  113   74  146  245   34  255   96   24<br />
        /// 232  255   23    9  167  240  255   44  194   98<br />
        /// 18  175  173  204  169  171  236  127  114   23<br />
        /// 167  202  132   65  253   11  254   56  214  127<br />
        /// 145  191  104  163  143    7  174  224  247   73<br />
        /// 52    6  231  255    5  101   83  165  160  231<br /><br />
        /// Second Series:<br />
        /// 97  129  149   54   22  208  120  105   68  177<br />
        /// 113  214   30  172   74  218  116  230   89   18<br />
        /// 12  112  130  105  116  180  190  200  187  120<br />
        /// 7  198  233  158   58   51   50  170   98   23<br />
        /// 21    1  113   74  146  245   34  255   96   24<br />
        /// 232  255   23    9  167  240  255   44  194   98<br />
        /// 18  175  173  204  169  171  236  127  114   23<br />
        /// 167  202  132   65  253   11  254   56  214  127<br />
        /// 145  191  104  163  143    7  174  224  247   73<br />
        /// 52    6  231  255    5  101   83  165  160  231</remarks>
        public static void ExampleMain3()
        {
            byte[] bytes1 = new byte[100];
            byte[] bytes2 = new byte[100];

            int nSeed = rand.Next();
            
            Random rnd1 = new Random(nSeed);
            Random rnd2 = new Random(nSeed);

            rnd1.NextBytes(bytes1);
            rnd2.NextBytes(bytes2);
            
            WriteSeries("First Series",bytes1);

            Console.WriteLine();

            WriteSeries("Second Series:", bytes2);

            // The example displays output like the following:
            //       First Series:
            //          97  129  149   54   22  208  120  105   68  177
            //         113  214   30  172   74  218  116  230   89   18
            //          12  112  130  105  116  180  190  200  187  120
            //           7  198  233  158   58   51   50  170   98   23
            //          21    1  113   74  146  245   34  255   96   24
            //         232  255   23    9  167  240  255   44  194   98
            //          18  175  173  204  169  171  236  127  114   23
            //         167  202  132   65  253   11  254   56  214  127
            //         145  191  104  163  143    7  174  224  247   73
            //          52    6  231  255    5  101   83  165  160  231
            //       
            //       Second Series:
            //          97  129  149   54   22  208  120  105   68  177
            //         113  214   30  172   74  218  116  230   89   18
            //          12  112  130  105  116  180  190  200  187  120
            //           7  198  233  158   58   51   50  170   98   23
            //          21    1  113   74  146  245   34  255   96   24
            //         232  255   23    9  167  240  255   44  194   98
            //          18  175  173  204  169  171  236  127  114   23
            //         167  202  132   65  253   11  254   56  214  127
            //         145  191  104  163  143    7  174  224  247   73
            //          52    6  231  255    5  101   83  165  160  231        

        }

        /// <summary>
        /// Writes the series.
        /// </summary>
        /// <param name="sTitle">The s title.</param>
        /// <param name="bytes1">The bytes1.</param>
        private static void WriteSeries(string sTitle, byte[] bytes1)
        {
            Console.WriteLine(sTitle);
            for (int ctr = bytes1.GetLowerBound(0);
                 ctr <= bytes1.GetUpperBound(0);
                 ctr++)
            {
                Console.Write("{0, 5}", bytes1[ctr]);
                if ((ctr + 1) % 10 == 0) Console.WriteLine();
            }
        }
    }
}
