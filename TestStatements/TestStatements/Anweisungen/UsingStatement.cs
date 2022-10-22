// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-10-2022
// ***********************************************************************
// <copyright file="UsingStatement.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.IO;

namespace TestStatements.Anweisungen
{
    /// <summary>
    /// Class UsingStatement.
    /// </summary>
    public class UsingStatement
    {

        /// <summary>
        /// The filename
        /// (constant string)
        /// </summary>
        const string csFilename = "test.txt";
        /// <summary>
        /// The title (constant string)
        /// </summary>
        const string Title = "Beispiel für Using-Statement";
        /// <summary>
        /// Does the using statement.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void DoUsingStatement(string[] args)
        {
            CreateFile();
            ReadFile();
        }

        /// <summary>
        /// Reads the file.
        /// </summary>
        private static void ReadFile()
        {
            using (TextReader r = File.OpenText(csFilename))
            {
                Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));

                Console.WriteLine(r.ReadLine());
                Console.WriteLine(r.ReadLine());
                Console.WriteLine(r.ReadLine());
            }
        }

        /// <summary>
        /// Creates the file.
        /// </summary>
        private static void CreateFile()
        {
            using (TextWriter w = File.CreateText(csFilename))
            {
                w.WriteLine("Line one");
                w.WriteLine("Line two");
                w.WriteLine("Line three");
            }
        }
    }
}