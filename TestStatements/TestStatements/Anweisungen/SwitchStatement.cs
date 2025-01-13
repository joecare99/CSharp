// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 07-13-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="SwitchStatement.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TestStatements.Anweisungen
{
    /// <summary>
    /// Class SwitchStatement.
    /// </summary>
    public class SwitchStatement
    {
        /// <summary>
        /// The r
        /// </summary>
        private static Random r = new Random();

        /// <summary>
        /// The get now
        /// </summary>
        public static Func<DateTime> GetNow = ()=> DateTime.Now; 
        /// <summary>
        /// The random
        /// </summary>
        public static Func<Random> random = ()=> r;


        /// <summary>
        /// Switches the example1.
        /// </summary>
        public static void SwitchExample1()
        {
            int caseSwitch = random().Next(3);

            switch (caseSwitch)
            {
                /// <summary>Enum Color</summary>
                case 1:
                    Console.WriteLine("Case 1");
                    break;
                case 2:
                    Console.WriteLine("Case 2");
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }
        }

        /// <summary>
        /// The enum Color
        /// </summary>
        public enum Color {
            /// <summary>
            /// The red
            /// </summary>
            Red,
            /// <summary>
            /// The green
            /// </summary>
            Green,
            /// <summary>
            /// The blue
            /// </summary>
            Blue
        }

        /// <summary>
        /// Switches the example2.
        /// </summary>
        public static void SwitchExample2()
        {
            Color c = (Color)(random().Next(-1, 3));
            switch (c)
            {
                case Color.Red:
                    Console.WriteLine("The color is red");
                    break;
                case Color.Green:
                    Console.WriteLine("The color is green");
                    break;
                case Color.Blue:
                    Console.WriteLine("The color is blue");
                    break;
                default:
                    Console.WriteLine("The color is unknown.");
                    break;
            }
        }

        /// <summary>
        /// Switches the example3.
        /// </summary>
        public static void SwitchExample3()
        {
            int caseSwitch = random().Next(0, 4);

            switch (caseSwitch)
            {
                case 1:
                    Console.WriteLine("Case 1");
                    break;
                case 2:
                case 3:
                    Console.WriteLine($"Case {caseSwitch}");
                    break;
                default:
                    Console.WriteLine($"An unexpected value ({caseSwitch})");
                    break;
            }
        }

        /// <summary>
        /// Switches the example4.
        /// </summary>
        public static void SwitchExample4()
        {
            var values = new List<object?>();
            for (int ctr = 0; ctr <= 7; ctr++)
            {
                if (ctr == 2)
                    values.Add(DiceLibrary.Roll2());
                else if (ctr == 4)
                    values.Add(DiceLibrary.Pass());
                else
                    values.Add(DiceLibrary.Roll());
            }

            Console.WriteLine($"The sum of { values.Count } die is { DiceLibrary.DiceSum(values) }");
        }

        /// <summary>
        /// Switches the example5.
        /// </summary>
        public static void SwitchExample5()
        {
            switch (GetNow().DayOfWeek)
            {
                case DayOfWeek.Sunday:
                case DayOfWeek.Saturday:
                    Console.WriteLine("The weekend");
                    break;
                case DayOfWeek.Monday:
                    Console.WriteLine("The first day of the work week.");
                    break;
                case DayOfWeek.Friday:
                    Console.WriteLine("The last day of the work week.");
                    break;
                default:
                    Console.WriteLine("The middle of the work week.");
                    break;
            }
        }
        /// <summary>
        /// Switches the example6.
        /// </summary>
        public static void SwitchExample6()
        {
            int[] values = { 2, 4, 6, 8, 10 };
            ShowCollectionInformation(values);

            var names = new List<string>();
            names.AddRange(new string[] { "Adam", "Abigail", "Bertrand", "Bridgette" });
            ShowCollectionInformation(names);

			List<int>? numbers = null;

			ShowCollectionInformation(numbers);
        }

        /// <summary>
        /// Show Collection Information
        /// </summary>
        /// <param name="coll">The coll.</param>
        private static void ShowCollectionInformation(object? coll)

		{
			switch (coll)
            {
                case Array arr:
                    Console.WriteLine($"An array with {arr.Length} elements.");
                    break;
                case IEnumerable<int> ieInt:
                    Console.WriteLine($"Average: {ieInt.Average(s => s)}");
                    break;
                case IList list:
                    Console.WriteLine($"{list.Count} items");
                    break;
                case IEnumerable ie:
                    string result = "";
                    foreach (var e in ie)
                        result += $"{e} ";
                    Console.WriteLine(result);
                    break;
                case null:
                    // Do nothing for a null.
                    break;
                default:
                    Console.WriteLine($"A instance of type {coll.GetType().Name}");
                    break;
            }
        }

        /// <summary>
        /// Switches the example7.
        /// </summary>
        public static void SwitchExample7()
        {
            int[] values = { 2, 4, 6, 8, 10 };
            ShowCollectionInformation2(values);

            var names = new List<string>();
            names.AddRange(new string[] { "Adam", "Abigail", "Bertrand", "Bridgette" });
            ShowCollectionInformation2(names);

#if NET5_0_OR_GREATER
			List<int>? numbers = null;
#else
			List<int> numbers = null;
#endif
			ShowCollectionInformation2(numbers);
        }

        /// <summary>
        /// Shows the collection information2.
        /// </summary>
        /// <typeparam name="T">The generic Type</typeparam>
        /// <param name="coll">The collection.</param>
        private static void ShowCollectionInformation2<T>(T coll)
        {
            switch (coll)
            {
                case Array arr:
                    Console.WriteLine($"An array with {arr.Length} elements.");
                    break;
                case IEnumerable<int> ieInt:
                    Console.WriteLine($"Average: {ieInt.Average(s => s)}");
                    break;
                case IList list:
                    Console.WriteLine($"{list.Count} items");
                    break;
                case IEnumerable ie:
                    string result = "";
                    foreach (var e in ie)
                        result += $"{e} ";
                    Console.WriteLine(result);
                    break;
                case object o:
                    Console.WriteLine($"A instance of type {o.GetType().Name}");
                    break;
                default:
                    Console.WriteLine("Null passed to this method.");
                    break;
            }
        }

    }

    /// <summary>
    /// Class DiceLibrary.
    /// </summary>
    public static class DiceLibrary
    {
        // Random number generator to simulate dice rolls.
        /// <summary>
        /// The random
        /// </summary>
        static Random rnd = SwitchStatement.random();

        // Roll a single die.
        /// <summary>
        /// Rolls this instance.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public static int Roll()
        {
            return rnd.Next(1, 7);
        }

        // Roll two dice.
        /// <summary>
        /// Roll2s this instance.
        /// </summary>
        /// <returns>List&lt;System.Object&gt;.</returns>
        public static List<object> Roll2()
        {
            var rolls = new List<object>();
            rolls.Add(Roll());
            rolls.Add(Roll());
            return rolls;
        }

        // Calculate the sum of n dice rolls.
        /// <summary>
        /// Dices the sum.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="InvalidOperationException">unknown item type</exception>
        public static int DiceSum(IEnumerable<object> values)
        {
            var sum = 0;
            foreach (var item in values)
            {
                switch (item)
                {
                    // A single zero value.
                    case 0:
                        break;
                    // A single value.
                    case int val:
                        sum += val;
                        break;
                    // A non-empty collection.
                    case IEnumerable<object> subList when subList.Any():
                        sum += DiceSum(subList);
                        break;
                    // An empty collection.
                    case IEnumerable<object> subList:
                        break;
                    //  A null reference.
                    case null:
                        break;
                    // A value that is neither an integer nor a collection.
                    default:
                        throw new InvalidOperationException("unknown item type");
                }
            }
            return sum;
        }

        /// <summary>
        /// Passes this instance.
        /// </summary>
        /// <returns>System.Object.</returns>
        public static object? Pass()
        {
            if (rnd.Next(0, 2) == 0)
                return null;
            else
                return new List<object>();
        }

    }
}
