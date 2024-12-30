// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="EnumTest.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace TestStatements.DataTypes
{
    /// <summary>
    /// Class EnumTest.
    /// </summary>
    public static class EnumTest
    {
        /// <summary>
        /// Enum name of days
        /// </summary>
        enum Days {
            /// <summary>
            /// The saturday
            /// </summary>
            Saturday,
            /// <summary>
            /// The sunday
            /// </summary>
            Sunday,
            /// <summary>
            /// The monday
            /// </summary>
            Monday,
            /// <summary>
            /// The tuesday
            /// </summary>
            Tuesday,
            /// <summary>
            /// The wednesday
            /// </summary>
            Wednesday,
            /// <summary>
            /// The thursday
            /// </summary>
            Thursday,
            /// <summary>
            /// The friday
            /// </summary>
            Friday
        };
        /// <summary>
        /// Enum boiling Points
        /// </summary>
        enum BoilingPoints 
        {
            /// <summary>
            /// The poinlin point of water in Celcius
            /// </summary>
            Celsius = 100,
            /// <summary>
            /// The poinlin point of water in Fahrenheit
            /// </summary>
            Fahrenheit = 212 
        };

        /// <summary>
        /// Enum Colors
        /// </summary>
        [Flags]
        enum Colors {
            /// <summary>
            /// The red
            /// </summary>
            Red = 1,
            /// <summary>
            /// The green
            /// </summary>
            Green = 2,
            /// <summary>
            /// The blue
            /// </summary>
            Blue = 4,
            /// <summary>
            /// The yellow
            /// </summary>
            Yellow = 8 
        };

        /// <summary>
        /// Mains the test.
        /// </summary>
        public static void MainTest()
        {
            const string Title = "Enumerations";
            Console.WriteLine(Constants.Constants.Header, Title);

            Type weekdays = typeof(Days);
            Type boiling = typeof(BoilingPoints);

            Console.WriteLine("The days of the week, and their corresponding values in the Days Enum are:");

            foreach (string s in Enum.GetNames(weekdays))
                Console.WriteLine("{0,-11}= {1}", s, Enum.Format(weekdays, Enum.Parse(weekdays, s), "d"));

            Console.WriteLine();
            Console.WriteLine("Enums can also be created which have values that represent some meaningful amount.");
            Console.WriteLine("The BoilingPoints Enum defines the following items, and corresponding values:");

            foreach (string s in Enum.GetNames(boiling))
                Console.WriteLine("{0,-11}= {1}", s, Enum.Format(boiling, Enum.Parse(boiling, s), "d"));

            Colors myColors = Colors.Red | Colors.Blue | Colors.Yellow;
            Console.WriteLine();
            Console.WriteLine("myColors holds a combination of colors. Namely: {0}", myColors);
        }
    }
}
