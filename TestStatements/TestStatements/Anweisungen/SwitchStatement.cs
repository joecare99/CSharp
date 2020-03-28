using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStatements.Anweisungen
{
    public class SwitchStatement
    {
        private static Random r = new Random();

        public static Func<DateTime> GetNow = delegate { return DateTime.Now; };
        public static Func<Random> random = delegate { return r; };


        public static void SwitchExample1()
        {
            int caseSwitch = 1;

            switch (caseSwitch)
            {
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

        public enum Color { Red, Green, Blue }

        public static void SwitchExample2()
        {
            Color c = (Color)(random().Next(0, 3));
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

        public static void SwitchExample3()
        {
            Random rnd = random();
            int caseSwitch = rnd.Next(1, 4);

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

        public static void SwitchExample4()
        {
            var values = new List<object>();
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
        public static void SwitchExample6()
        {
            int[] values = { 2, 4, 6, 8, 10 };
            ShowCollectionInformation(values);

            var names = new List<string>();
            names.AddRange(new string[] { "Adam", "Abigail", "Bertrand", "Bridgette" });
            ShowCollectionInformation(names);

            List<int> numbers = null;
            ShowCollectionInformation(numbers);
        }

        private static void ShowCollectionInformation(object coll)
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

        public static void SwitchExample7()
        {
            int[] values = { 2, 4, 6, 8, 10 };
            ShowCollectionInformation2(values);

            var names = new List<string>();
            names.AddRange(new string[] { "Adam", "Abigail", "Bertrand", "Bridgette" });
            ShowCollectionInformation2(names);

            List<int> numbers = null;
            ShowCollectionInformation2(numbers);
        }

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

    public static class DiceLibrary
    {
        // Random number generator to simulate dice rolls.
        static Random rnd = SwitchStatement.random();

        // Roll a single die.
        public static int Roll()
        {
            return rnd.Next(1, 7);
        }

        // Roll two dice.
        public static List<object> Roll2()
        {
            var rolls = new List<object>();
            rolls.Add(Roll());
            rolls.Add(Roll());
            return rolls;
        }

        // Calculate the sum of n dice rolls.
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

        public static object Pass()
        {
            if (rnd.Next(0, 2) == 0)
                return null;
            else
                return new List<object>();
        }

    }
}
