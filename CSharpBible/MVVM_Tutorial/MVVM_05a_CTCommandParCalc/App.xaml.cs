// ***********************************************************************
// Assembly         : MVVM_05a_CTCommandParCalc
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
using BaseLib.Helper;
using Microsoft.Extensions.DependencyInjection;
using BaseLib.Helper;
using MVVM_05a_CTCommandParCalc.Model;
using MVVM_05a_CTCommandParCalc.Model.Interfaces;
using System.Windows;

namespace MVVM_05a_CTCommandParCalc;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var sc = new ServiceCollection()
            .AddTransient<ICalculatorModel, CalculatorModel>();

        var bc = sc.BuildServiceProvider();

        IoC.Configure(bc);
    }

}
