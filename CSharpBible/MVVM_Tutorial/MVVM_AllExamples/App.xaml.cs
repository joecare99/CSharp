// ***********************************************************************
// Assembly         : MVVM_AllExamples
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
using Microsoft.Extensions.DependencyInjection;
using MVVM.View.Extension;
using System.Windows;

namespace MVVM_AllExamples
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var sc = new ServiceCollection()
                .AddSingleton<MVVM_22_WpfCap.Model.IWpfCapModel, MVVM_22_WpfCap.Model.CWpfCapModel>()
                .AddSingleton<MVVM_22_CTWpfCap.Model.IWpfCapModel, MVVM_22_CTWpfCap.Model.CWpfCapModel>()
                .AddTransient<MVVM_22_WpfCap.ViewModel.WpfCapViewModel>()
                .AddTransient<MVVM_22_CTWpfCap.ViewModel.WpfCapViewModel>()
                .AddTransient<MVVM_22_WpfCap.Model.IRandom, MVVM_22_WpfCap.Model.CRandom>()
                .AddTransient<MVVM_22_CTWpfCap.Model.IRandom, MVVM_22_CTWpfCap.Model.CRandom>();
            var sp = sc.BuildServiceProvider();
            IoC.Configure(sp);
        }
    }
}
namespace MVVM_AllExamples.Models { }
namespace MVVM_AllExamples.ValueConverter { }
namespace MVVM_AllExamples.Services { }
