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
using CommunityToolkit.Mvvm.Messaging;
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
                .AddTransient<BaseLib.Interfaces.ISysTime,BaseLib.Helper.SysTime>()
                .AddSingleton<MVVM_22_WpfCap.Model.IWpfCapModel, MVVM_22_WpfCap.Model.CWpfCapModel>()
                .AddSingleton<MVVM_22_CTWpfCap.Model.IWpfCapModel, MVVM_22_CTWpfCap.Model.CWpfCapModel>()
                .AddTransient<MVVM_22_WpfCap.ViewModels.WpfCapViewModel>()
                .AddTransient<MVVM_22_CTWpfCap.ViewModels.WpfCapViewModel>()
                .AddTransient<MVVM_22_WpfCap.Model.IRandom, MVVM_22_WpfCap.Model.CRandom>()
                .AddTransient<MVVM_22_CTWpfCap.Model.IRandom, MVVM_22_CTWpfCap.Model.CRandom>()
                .AddSingleton<MVVM_36_ComToolKtSavesWork.Models.IUserRepository, MVVM_36_ComToolKtSavesWork.Models.UserRepository>()
                .AddSingleton<MVVM_36_ComToolKtSavesWork.Models.ICommunityToolkit2Model, MVVM_36_ComToolKtSavesWork.Models.CommunityToolkit2Model>()
                .AddSingleton<IMessenger>(WeakReferenceMessenger.Default)
                .AddTransient<MVVM_36_ComToolKtSavesWork.ViewModels.MainWindowViewModel>()
                .AddTransient<MVVM_36_ComToolKtSavesWork.ViewModels.CommunityToolkit2ViewModel>()
                .AddTransient<MVVM_36_ComToolKtSavesWork.ViewModels.UserInfoViewModel>()
                .AddTransient<MVVM_36_ComToolKtSavesWork.ViewModels.LoginViewModel>()
                .AddSingleton<MVVM_37_TreeView.Services.IBooksService, MVVM_37_TreeView.Services.BooksService>()
                .AddTransient<MVVM_37_TreeView.ViewModels.MainWindowViewModel>()
                .AddTransient<MVVM_37_TreeView.ViewModels.BooksTreeViewModel>()
                .AddSingleton<MVVM_38_CTDependencyInjection.Models.Interfaces.IUserRepository, MVVM_38_CTDependencyInjection.Models.UserRepository>()
                .AddSingleton<MVVM_38_CTDependencyInjection.Models.Interfaces.ITemplateModel, MVVM_38_CTDependencyInjection.Models.TemplateModel>()
                .AddTransient<MVVM_38_CTDependencyInjection.Models.Interfaces.ILog, MVVM_38_CTDependencyInjection.Models.CLog>()
                .AddTransient<MVVM_38_CTDependencyInjection.Models.Interfaces.ITimer, MVVM_38_CTDependencyInjection.Models.CTimer>()
                .AddTransient<MVVM_38_CTDependencyInjection.Models.Interfaces.ISysTime, MVVM_38_CTDependencyInjection.Models.CSysTime>()
                .AddTransient<MVVM_38_CTDependencyInjection.ViewModels.MainWindowViewModel>()
                .AddTransient<MVVM_38_CTDependencyInjection.ViewModels.DependencyInjectionViewModel>()
                .AddSingleton<BaseLib.Interfaces.ILog,MVVM_40_Wizzard.Models.SimpleLog>()

                .AddSingleton<MVVM_39_MultiModelTest.Models.ISystemModel, MVVM_39_MultiModelTest.Models.SystemModel>()
                .AddScoped<MVVM_39_MultiModelTest.Models.IScopedModel, MVVM_39_MultiModelTest.Models.ScopedModel>()

                .AddSingleton<MVVM_40_Wizzard.Models.IWizzardModel, MVVM_40_Wizzard.Models.WizzardModel>()
                ;


            var sp = sc.BuildServiceProvider();
            IoC.Configure(sp);
        }
    }
}
namespace MVVM_AllExamples.Models { }
namespace MVVM_AllExamples.ValueConverter { }
namespace MVVM_AllExamples.Services { }
