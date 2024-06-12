// ***********************************************************************
// Assembly         : MVVM_40_Wizzard
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-24-2022
// ***********************************************************************
// <copyright file="App.xaml.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.Extensions.DependencyInjection;
using MVVM_40_Wizzard.Models;
using BaseLib.Interfaces;
using BaseLib.Helper;
using System.Windows;
using MVVM.View.Extension;

namespace MVVM_40_Wizzard;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {

        IServiceCollection services = new ServiceCollection()
            .AddSingleton<IWizzardModel, WizzardModel>()
            .AddTransient<ISysTime, SysTime>()
            .AddSingleton<ILog, SimpleLog>();

        ServiceProvider serviceProvider = services.BuildServiceProvider();

        IoC.Configure(serviceProvider);

        base.OnStartup(e);
    }
}
