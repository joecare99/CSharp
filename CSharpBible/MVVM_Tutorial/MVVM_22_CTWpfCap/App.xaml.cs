// ***********************************************************************
// Assembly         : MVVM_22_CTWpfCap
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
using BaseLib.Helper;
using BaseLib.Models;
using BaseLib.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using MVVM_22_CTWpfCap.Model;
using MVVM_22_CTWpfCap.ViewModels;
using MVVM_22_CTWpfCap.ViewModels.Factories;
using MVVM_22_CTWpfCap.ViewModels.Interfaces;
using System.Windows;

namespace MVVM_22_CTWpfCap;

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
        services.AddTransient<IWpfCapViewModel, WpfCapViewModel>();
        services.AddTransient<IRowDataFactory, RowDataFactory>();
        services.AddTransient<IColDataFactory, ColDataFactory>();

        ServiceProvider container = services.BuildServiceProvider();
        IoC.GetReqSrv = (type) => container.GetRequiredService(type);
        IoC.GetSrv = (type) => container.GetService(type);
    }
}
