// ***********************************************************************
// Assembly         : Calc64WF
// Author           : Mir
// Created          : 08-28-2022
//
// Last Modified By : Mir
// Last Modified On : 08-28-2022
// ***********************************************************************
// <copyright file="Program.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Windows.Forms;
using Calc64Base.Models;
using Calc64Base.Models.Interfaces;
using Calc64WF.ViewModel;
using Calc64WF.ViewModels.Interfaces;
using Calc64WF.Visual;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Calc64WF
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
        static void Main()
        {
            Init();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(Ioc.Default.GetService<Form>()!);
        }

        private static void Init()
        {
            var sp = new ServiceCollection()
             .AddSingleton<ICalculator, Calc64Model>()
             .AddTransient<IFrmCalc64MainViewModel, FrmCalc64MainViewModel>()
             .AddTransient<Form, FrmCalc64Main>()
             //   .AddTransient<Views.LoadingDialog, Views.LoadingDialog>()
             .BuildServiceProvider();

            Ioc.Default.ConfigureServices(sp);
        }
    }
}
