// ***********************************************************************
// Assembly         : MVVM_00_IoCTemplate
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
using BaseLib.Interfaces;
using BaseLib.Models;
using BaseLib.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using MVVM.View.Extension;
using MVVM_00_IoCTemplate.Models;
using System.Windows;

namespace MVVM_00_IoCTemplate
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var sc = new ServiceCollection()
                .AddTransient<ISysTime, SysTime>()
                .AddTransient<ILog, SimpleLog>()
                .AddSingleton<ITemplateModel, TemplateModel>();

            var bc = sc.BuildServiceProvider();

            IoC.Configure(bc);
        }
    }
}
namespace MVVM_00_IoCTemplate.Models { }
namespace MVVM_00_IoCTemplate.ValueConverter { }
namespace MVVM_00_IoCTemplate.Services { }
