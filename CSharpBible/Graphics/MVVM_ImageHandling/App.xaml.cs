// ***********************************************************************
// Assembly         : MVVM_ImageHandling
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
using BaseLib.Models;
using BaseLib.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using MVVM_ImageHandling.Models;
using System.Windows;

namespace MVVM_ImageHandling
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var sc = new ServiceCollection()
                .AddSingleton<IImageHandlingModel, ImageHandlingModel>()
                .AddTransient<ICyclTimer, TimerProxy>()
                .AddTransient<ISysTime, SysTime>();
            var sp = sc.BuildServiceProvider();

            IoC.Configure(sp);
            base.OnStartup(e);
        }
    }
}
namespace MVVM_ImageHandling.Models { }
namespace MVVM_ImageHandling.ValueConverter { }
namespace MVVM_ImageHandling.Services { }
