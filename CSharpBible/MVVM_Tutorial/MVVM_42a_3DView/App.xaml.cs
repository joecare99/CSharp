// ***********************************************************************
// Assembly         : MVVM_42a_3DView
// Author           : Mir
// Created          : 03-09-2025
//
// Last Modified By : Mir
// Last Modified On : 03-09-2025
// ***********************************************************************
// <copyright file="App.xaml.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Helper;
using BaseLib.Models;
using BaseLib.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using MVVM_42a_3DView.Models;
using System.Windows;

namespace MVVM_42a_3DView
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
                .AddSingleton<ISceneModel, SceneModel>();

            var bc = sc.BuildServiceProvider();

            IoC.Configure(bc);
        }
    }
}
namespace MVVM_42a_3DView.Models { }
namespace MVVM_42a_3DView.ValueConverter { }
namespace MVVM_42a_3DView.Services { }
