// ****************.*******************************************************
// Assembly         : Permutation
// Author           : Mir
// Created          : 07-30-2022
//
// Last Modified By : Mir
// Last Modified On : 07-30-2022
// ***********************************************************************
// <copyright file="Program.cs" company="Permutation">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Permutation.Model;
using System;
using System.Threading;

namespace Permutation
{
    /// <summary>
    /// Class Program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            var pr100_1 = new PermutatedRange(625,625);
            var pr200_1 = new PermutatedRange(1250,625);
            int s1=0,s2 = 0,s3 = 0,s4 = 0,s5=0;
            for (int i = 0; i < 625; i++)
            {
                s1 += pr100_1[i];
                s2 += pr200_1[i * 2];
                s3 += pr200_1[i * 2+1];
                s4 += pr200_1[i];
                s5 += pr200_1[i + 625];
            }

            Console.WriteLine($"{s1}\t{s2}\t{s3}\t{s4}\t{s5}");
            for (int i = 0; i < 625; i++)
            {
                Console.SetCursorPosition((pr100_1[i] % 25) * 4, pr100_1[i] / 25 + 2);
                Console.Write(i);
                Thread.Sleep(30);
            }
            Console.SetCursorPosition(0, 27);
        }
    }
}