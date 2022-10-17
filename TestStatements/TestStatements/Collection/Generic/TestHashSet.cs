// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-10-2022
// ***********************************************************************
// <copyright file="TestHashSet.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace TestStatements.Collection.Generic
{
    /// <summary>
    /// Class TestHashSet.
    /// </summary>
    public static class TestHashSet
    {
        /// <summary>
        /// Shows the hash set.
        /// </summary>
        /// <remarks>This example produces output similar to the following:
        /// <list type="bullet"><item>
        /// evenNumbers contains 5 elements: { 0 2 4 6 8 }
        /// </item><item>
        /// oddNumbers contains 5 elements: { 1 3 5 7 9 }
        /// </item><item>
        /// numbers UnionWith oddNumbers...     </item><item>
        /// numbers contains 10 elements: { 0 2 4 6 8 1 3 5 7 9 }
        /// </item></list></remarks>

        static public void ShowHashSet()
        {
            const string Title = "Show HashSet<T>";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));

            HashSet<int> evenNumbers, oddNumbers;
            InitializeTestData();

            Console.Write("evenNumbers contains {0} elements: ", evenNumbers.Count);
            DisplaySet(evenNumbers);

            Console.Write("oddNumbers contains {0} elements: ", oddNumbers.Count);
            DisplaySet(oddNumbers);

            // Create a new HashSet populated with even numbers.
            HashSet<int> numbers = new HashSet<int>(evenNumbers);
            Console.WriteLine("numbers UnionWith oddNumbers...");
            numbers.UnionWith(oddNumbers);

            Console.Write("numbers contains {0} elements: ", numbers.Count);
            DisplaySet(numbers);

            void InitializeTestData()
            {
                evenNumbers = new HashSet<int>();
                oddNumbers = new HashSet<int>();
                for (int i = 0; i < 5; i++)
                {
                    // Populate numbers with just even numbers.
                    evenNumbers.Add(i * 2);

                    // Populate oddNumbers with just odd numbers.
                    oddNumbers.Add((i * 2) + 1);
                }
            }

            /* This example produces output similar to the following:
            * evenNumbers contains 5 elements: { 0 2 4 6 8 }
            * oddNumbers contains 5 elements: { 1 3 5 7 9 }
            * numbers UnionWith oddNumbers...
            * numbers contains 10 elements: { 0 2 4 6 8 1 3 5 7 9 }
            */
        }

        /// <summary>
        /// Displays the set.
        /// </summary>
        /// <param name="collection">The collection.</param>
        private static void DisplaySet(HashSet<int> collection)
        {
            Console.Write("{");
            foreach (int i in collection)
            {
                Console.Write(" {0}", i);
            }
            Console.WriteLine(" }");
        }
    }
}
