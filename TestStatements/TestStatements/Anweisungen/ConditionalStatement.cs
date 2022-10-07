// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 07-13-2022
//
// Last Modified By : Mir
// Last Modified On : 09-10-2022
// ***********************************************************************
// <copyright file="ConditionalStatement.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace TestStatements.Anweisungen
{
    /// <summary>
    /// Example class for if statement
    /// </summary>
    public class ConditionalStatement
    {
        /// <summary>
        /// Does an if statement.
        /// branches if the length of <see cref="args" /> is 0
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void DoIfStatement(string[] args)
        {
            const string Title = "Auswertung von Bedingungen (IF-Anweisung)";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));
            if (args.Length == 0)
            {
                Console.WriteLine("No arguments");
            }
            else
            {
                Console.WriteLine("One or more arguments");
            }
        }

        /// <summary>
        /// Does two nested if statements.
        /// <br /> the first if statement checks if there are more than 0 arguments in <see cref="args" />.<br />
        /// the second if statment checks if there are more than 1 arguments in <see cref="args" />
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void DoIfStatement2(string[] args)
        {
            const string Title = "Auswertung von Bedingungen (IF-Anweisung) 2";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));
            if (args.Length >= 0)
                if (args.Length >= 1)
                    DoIt();
                else
                {
                    Console.WriteLine("one arguments");
                }
            else
            {
                Console.WriteLine("No arguments");
            }
        }

        /// <summary>
        /// Does several if statements.
        /// </summary>
        /// <remarks>if without statements, just a semicolon.<br />
        /// if with just empty curly brackets.<br />
        /// if calling <see cref="DoIt()" /> directly.<br />
        /// if with curly brackets calling <see cref="DoIt()" /> inside.<br />
        /// nested if calling <see cref="DoIt()" /> on else of first if<br />
        /// curly brackets inside if calling <see cref="DoIt()" /> directly with empty else.</remarks>
        public static void DoIfStatement3()
        {
            const string Title = "Auswertung von Bedingungen (IF-Anweisung) 3";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));
#pragma warning disable CS0642 // Möglicherweise falsche leere Anweisung(a) ; // Warning but OK in syntax 
            //---------------
            if (a) ; // Warning but OK in syntax 
#pragma warning restore CS0642 // Möglicherweise falsche leere Anweisung
            //---------------
            if (a) { }
            //---------------
            if (a) DoIt();
            //---------------
            if (a) { DoIt(); }
            //---------------
            if (a)
#pragma warning disable CS0642 // Möglicherweise falsche leere Anweisung
                if (b) ; // Warning but OK in syntax 
                else; // Warning but OK in syntax
#pragma warning restore CS0642 // Möglicherweise falsche leere Anweisung
            else DoIt();
            { }
#pragma warning disable CS0642 // Möglicherweise falsche leere Anweisung
            { if (a) DoIt(); else; } // Warning but OK in syntax
#pragma warning restore CS0642 // Möglicherweise falsche leere Anweisung
            DoIt();
        }


        /// <summary>
        /// Does the switch statement.<br />"simple" switch with <see cref="T:System.Int32">Int32</see> number with the number of argumemts in <see cref="args" />
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <remarks>Prints "No arguments" if there are no arguments in args<br />
        /// Prints "One argument" if there is one argument in args.<br />
        /// Prints "2 arguments" if there are two arguments <br />
        /// Prints "3 arguments" if there are three arguments <br />
        /// ...<br />
        /// Prints "n arguments" if there are n arguments <br /></remarks>
        public static void DoSwitchStatement(string[] args)
        {
            const string Title = "Auswertung von Bedingungen (SWITCH-Anweisung)";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));

            int n = args.Length;
            switch (n)
            {
                case 0:
                    Console.WriteLine("No arguments");
                    break;
                case 1:
                    Console.WriteLine("One argument");
                    break;
                default:
                    Console.WriteLine($"{n} arguments");
                    break;
            }
        }

        /// <summary>
        /// Does some special switch statement.
        /// </summary>
        /// <remarks><list type="number">
        ///   <item>an empty switch-block</item>
        ///   <item>switch block with only one case</item>
        ///   <item>switch block with two cases executing the same statement</item>
        ///   <item>switch bloch with 4 cases and different break-settings</item>
        /// </list></remarks>
        public static void DoSwitchStatement1()
        {
            const string Title = "Auswertung von Bedingungen (SWITCH-Anweisung) 2";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));


            int n = 4;
            //Empty Switch ----------------------
            switch (n)
#pragma warning disable CS1522 // Leerer Schalterblock.
            {  // Gives warning but is Compilable
#pragma warning restore CS1522 // Leerer Schalterblock.
            }
            //----------------------
            switch (n)
            {
                case 1:break;
            }

            switch (n)
            {
                case 0:
                case 1: break;
            }

            switch (n)
            {
#pragma warning disable CS0162 // Unerreichbarer Code wurde entdeckt.
                case 1: { break; DoIt(); }
                case 2: { if (a) break; else break; DoIt(); }
                case 3: break; DoIt();
                case 4: { return; }
#pragma warning restore CS0162 // Unerreichbarer Code wurde entdeckt.
            }

		}

        /// <summary>
        /// Does it.
        /// </summary>
        private static void DoIt()
        {
            Console.WriteLine("Done ...");
        }

        /// <summary>
        /// a
        /// </summary>
        private static bool a = false;
        /// <summary>
        /// The b
        /// </summary>
        private static bool b = true;
    }
}
