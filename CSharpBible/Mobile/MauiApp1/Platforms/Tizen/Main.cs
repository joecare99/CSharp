// ***********************************************************************
// Assembly         : MauiApp1
// Author           : Mir
// Created          : 08-31-2022
//
// Last Modified By : Mir
// Last Modified On : 08-31-2022
// ***********************************************************************
// <copyright file="Main.cs" company="MauiApp1">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using System;

namespace MauiApp1
{
/// <summary>
/// Class Program.
/// </summary>
    internal class Program : MauiApplication
    {
        /// <summary>
        /// Creates the maui application.
        /// </summary>
        /// <returns>MauiApp.</returns>
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            var app = new Program();
            app.Run(args);
        }
    }
}