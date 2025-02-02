// ***********************************************************************
// Assembly         : MVVM_28_1_CTDataGridExt
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
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using BaseLib.Interfaces;
using BaseLib.Helper;
using MVVM_28_1_CTDataGridExt.Services;
using BaseLib.Models;
using BaseLib.Models.Interfaces;

namespace MVVM_28_1_CTDataGridExt;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var sp = new ServiceCollection()
            .AddTransient<IRandom, CRandom>()
            .AddSingleton<IPersonService, PersonService>()

         .BuildServiceProvider();
        
        IoC.Configure(sp);
    }
}
