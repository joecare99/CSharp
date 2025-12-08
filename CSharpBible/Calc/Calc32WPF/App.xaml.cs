// ***********************************************************************
// Assembly         : Calc32WPF_net
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 10-22-2022
// ***********************************************************************
// <copyright file="App.xaml.cs" company="Calc32WPF_net">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Calc32.Models;
using Calc32.Models.Interfaces;
using Calc32.ViewModels;
using Calc32.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

/// <summary>
/// The Calc32WPF namespace.
/// </summary>
namespace Calc32WPF
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var sp = new ServiceCollection()
 .AddSingleton<ICalculatorClass, CalculatorClass>()
 .AddTransient<ICalculatorViewModel, CalculatorViewModel>()
 //   .AddTransient<Views.LoadingDialog, Views.LoadingDialog>()
 .BuildServiceProvider();

            Ioc.Default.ConfigureServices(sp);

            base.OnStartup(e);
        }
    }
}
