﻿// ***********************************************************************
// Assembly         : Display_Test
// Author           : Mir
// Created          : 07-15-2022
//
// Last Modified By : Mir
// Last Modified On : 11-05-2022
// ***********************************************************************
// <copyright file="Program.cs" company="JC-Soft">
//     Copyright © 2022 by JC-Soft
// </copyright>
// <summary></summary>
// ***********************************************************************
using ConsoleDisplay.View;
using DisplayTest.Models.Interfaces;
using DisplayTest.Models;
using System;
using System.Threading;
using System.Xml.Schema;
using BaseLib.Models.Interfaces;

/// <summary>
/// The Display_Test namespace.
/// </summary>
/// <autogeneratedoc />
namespace Display_Test
{
    /// <summary>
    /// Class Program.
    /// </summary>
    public static class Program 
    {
        private static IDisplayTest model;
        private static IRandom random;

        public static Func<IDisplayTest> GetModel = () => new DisplayTest.Models.DisplayTest();
        public static Func<IRandom> GetRandom = () => new BaseLib.Models.CRandom();

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static public void Main(string[] args)
        {
            Init(args);
            Run();
        }

        private static void Run()
        {
            model.DisplayTest1(random);
            model.DisplayTest2();
            model.DisplayTest3(random);
        }

        private static void Init(string[] args)
        {
            model = GetModel();
            random = GetRandom();
        }
    }
}