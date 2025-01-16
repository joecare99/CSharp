// ***********************************************************************
// Assembly         : MVVM_22_WpfCap
// Author           : Mir
// Created          : 08-14-2022
//
// Last Modified By : Mir
// Last Modified On : 08-14-2022
// ***********************************************************************
// <copyright file="App.xaml.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using BaseLib.Helper;
using BaseLib.Interfaces;
using MVVM.View.Extension;
using MVVM_22_WpfCap.Model;
using MVVM_22_WpfCap.ViewModels;

namespace MVVM_22_WpfCap;

/// <summary>
/// Interaktionslogik für "App.xaml"
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        //...
        var services = new ServiceCollection();
        services.AddTransient<IWpfCapModel, CWpfCapModel>();
        services.AddTransient<IRandom, CRandom>();
        services.AddTransient<WpfCapViewModel, WpfCapViewModel>();

        ServiceProvider container = services.BuildServiceProvider();
        IoC.GetReqSrv = (type) => container.GetRequiredService(type);
        IoC.GetSrv = (type) => container.GetService(type);
    }
}
