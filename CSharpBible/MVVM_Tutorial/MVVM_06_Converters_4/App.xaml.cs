// ***********************************************************************
// Assembly         : MVVM_6_Converters_4
// Author           : Mir
// Created          : 07-03-2022
//
// Last Modified By : Mir
// Last Modified On : 07-04-2022
// ***********************************************************************
// <copyright file="App.xaml.cs" company="MVVM_6_Converters_4">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Helper;
using Microsoft.Extensions.DependencyInjection;
using BaseLib.Helper;
using MVVM_06_Converters_4.Model;
using System.Windows;

namespace MVVM_06_Converters_4;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var sb = new ServiceCollection()
            .AddSingleton<IAGVModel, AGV_Model>()
            .AddSingleton<ViewModels.VehicleViewModel>()
            .BuildServiceProvider();
       IoC.Configure(sb);
    }
}
