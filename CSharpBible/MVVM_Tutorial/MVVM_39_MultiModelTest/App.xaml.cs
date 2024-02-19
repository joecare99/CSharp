// ***********************************************************************
// Assembly         : MVVM_39_MultiModelTest
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
using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using MVVM.View.Extension;

namespace MVVM_39_MultiModelTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            InitIoC();
        }

        public void InitIoC()
        {
            ServiceCollection services = new ServiceCollection();

            services.AddSingleton<Models.ISystemModel, Models.SystemModel>();
            services.AddScoped<Models.IScopedModel,Models.ScopedModel>();

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            IoC.Configure(serviceProvider);
        }
    }
}
namespace MVVM_39_MultiModelTest.Models { }
namespace MVVM_39_MultiModelTest.ValueConverter { }
namespace MVVM_39_MultiModelTest.Services { }
