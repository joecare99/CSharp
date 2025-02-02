// ***********************************************************************
// Assembly         : MVVM_37_TreeView
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
using MVVM_37_TreeView.Services;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using MVVM_37_TreeView.ViewModels;
using BaseLib.Helper;

namespace MVVM_37_TreeView
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var sc = new ServiceCollection()
                .AddSingleton<IBooksService, BooksService>()
                .AddTransient<MainWindowViewModel>()
                .AddTransient<BooksTreeViewModel>()
                .BuildServiceProvider();
            IoC.GetReqSrv = sc.GetRequiredService;
            IoC.GetSrv = sc.GetService;
            base.OnStartup(e);
        }
    }
}
namespace MVVM_37_TreeView.Models { }
namespace MVVM_37_TreeView.ValueConverter { }
namespace MVVM_37_TreeView.Services { }
