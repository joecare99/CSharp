// ***********************************************************************
// Assembly         : WPF_Sample_Template
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
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Navigation;
using WPF_Sample_Template.Models;
using WPF_Sample_Template.ViewModels;

namespace WPF_Sample_Template
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var sc = new ServiceCollection()
                .AddSingleton<ITemplateModel, TemplateModel>()
                .AddTransient<TemplateViewModel, TemplateViewModel>();

            var sp = sc.BuildServiceProvider();

            Ioc.Default.ConfigureServices(sp);

            base.OnStartup(e);
        }
    }
}
namespace WPF_Sample_Template.Models { }
namespace WPF_Sample_Template.ValueConverter { }
namespace WPF_Sample_Template.Services { }
