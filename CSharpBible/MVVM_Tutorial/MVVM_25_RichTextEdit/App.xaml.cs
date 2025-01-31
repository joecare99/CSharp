// ***********************************************************************
// Assembly         : MVVM_25_RichTextEdit
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
using BaseLib.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using MVVM.View.Extension;
using MVVM_25_RichTextEdit.Models;
using System.Windows;

namespace MVVM_25_RichTextEdit
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
                .AddSingleton<IRichTextEditModel, RichTextEditModel>();

            var bc = sc.BuildServiceProvider();

            IoC.Configure(bc);
        }
    }
}
namespace MVVM_25_RichTextEdit.Models { }
namespace MVVM_25_RichTextEdit.ValueConverter { }
namespace MVVM_25_RichTextEdit.Services { }
