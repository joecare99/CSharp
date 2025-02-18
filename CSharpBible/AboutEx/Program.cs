// ***********************************************************************
// Assembly         : AboutEx
// Author           : Mir
// Created          : 11-11-2022
//
// Last Modified By : Mir
// Last Modified On : 02-18-2024
// ***********************************************************************
// <copyright file="Program.cs" company="HP Inc.">
//     Copyright (c) HP Inc.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Windows.Forms;
using BaseLib.Helper;
using CSharpBible.AboutEx.ViewModels;
using CSharpBible.AboutEx.ViewModels.Interfaces;
using CSharpBible.AboutEx.Visual;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// The AboutEx namespace.
/// </summary>
namespace CSharpBible.AboutEx
{
    /// <summary>
    /// Class Program.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Init(args);
            Application.Run(IoC.GetRequiredService<Form>());
        }

        private static void Init(string[] args)
        {            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var sc = new ServiceCollection()
                .AddTransient<FrmAbout>()
                .AddTransient<AboutBox1>()
                .AddTransient<Form, FrmAboutExMain>()
                .AddTransient<IAboutViewModel, AboutViewModel>()
                .AddTransient<IFrmAboutExMainViewModel, FrmAboutExMainViewModel>();
            var sp = sc.BuildServiceProvider();
            IoC.Configure(sp);
        }
    }
}
