// ***********************************************************************
// Assembly         : MVVM_38_CTDependencyInjection
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
using MVVM.View.Extension;
using MVVM_38_CTDependencyInjection.Models;
using MVVM_38_CTDependencyInjection.Models.Interfaces;
using MVVM_38_CTDependencyInjection.ViewModels;
using System.Windows;

namespace MVVM_38_CTDependencyInjection
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var srvProv = new ServiceCollection()
                .AddSingleton<IUserRepository, UserRepository>()
                .AddSingleton<ITemplateModel, TemplateModel>()
                .AddTransient<ILog, CLog>()
                .AddTransient<ITimer, CTimer>()
                .AddTransient<ISysTime, CSysTime>()
                .AddTransient<MainWindowViewModel>()
                .AddTransient<DependencyInjectionViewModel>()

                .BuildServiceProvider();
            IoC.GetReqSrv = srvProv.GetRequiredService;
            IoC.GetSrv = srvProv.GetService;
        }
    }
}
namespace MVVM_38_CTDependencyInjection.Models { }
namespace MVVM_38_CTDependencyInjection.ValueConverter { }
namespace MVVM_38_CTDependencyInjection.Services { }
