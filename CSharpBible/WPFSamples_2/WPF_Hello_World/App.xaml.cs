// ***********************************************************************
// Assembly         : WPF_Hello_World
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
using BaseLib.Models;
using BaseLib.Models.Interfaces;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using WPF_Hello_World.Models;
using WPF_Hello_World.Models.Interfaces;
using WPF_Hello_World.ViewModels;

namespace WPF_Hello_World
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var sc = new ServiceCollection()
    .AddSingleton<IHelloWorldModel, HelloWorldModel>()
    .AddTransient<ICyclTimer, TimerProxy>()
    .AddTransient<HelloWorldViewModel, HelloWorldViewModel>();

            var sp = sc.BuildServiceProvider();

            Ioc.Default.ConfigureServices(sp);


            base.OnStartup(e);
        }
    }
}
namespace WPF_Hello_World.Models { }
namespace WPF_Hello_World.ValueConverter { }
namespace WPF_Hello_World.Services { }
