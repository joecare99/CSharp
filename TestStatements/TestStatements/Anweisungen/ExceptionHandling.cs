// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="ExceptionHandling.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace TestStatements.Anweisungen
{
	/// <summary>
	/// Class ExceptionHandling.
	/// </summary>
	public class ExceptionHandling
    {
        /// <summary>
        /// Divides the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>System.Double.</returns>
        /// <exception cref="DivideByZeroException"></exception>
        static double Divide(double x, double y)
        {
            if (y == 0)
                throw new DivideByZeroException();
            return x / y;
        }

        /// <summary>
        /// Does the try catch.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <exception cref="InvalidOperationException">Two numbers required</exception>
        public static void DoTryCatch(string[] args)
        {
            try
            {
                if (args.Length != 2)
                {
                    throw new InvalidOperationException("Two numbers required");
                }
                double x = double.Parse(args[0]);
                double y = double.Parse(args[1]);
                Console.WriteLine(Divide(x, y));
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Console.WriteLine("Good bye!");
            }
        }

        /// <summary>
        /// Does the try finally.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <exception cref="InvalidOperationException">Two numbers required</exception>
        public static void DoTryFinally(string[] args) {
			try {
				if (args == null || args.Length == 0) {
					return;			
				}
				else
				if (args.Length == 1) {
					Console.WriteLine($"The parameter is: ({args[0]})");
					return;
				}
				else
				if (args.Length >= 2) {
					Console.WriteLine($"The first parameters are: ({args[0]}) and ({args[1]})");
					return;
				}
			}
			finally {
				Console.WriteLine("Good bye!");
			}
		}

	}
}
