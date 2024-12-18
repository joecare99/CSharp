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
using BaseLib.Helper;
using BaseLib.Helper.MVVM;
using BaseLib.Interfaces;

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
                .AddTransient<ISysTime,SysTime>()
                .AddTransient<IRandom,CRandom>()
                .AddSingleton<ILog,MVVM_40_Wizzard.Models.SimpleLog>()
                .AddSingleton<IMessenger>(WeakReferenceMessenger.Default)

                .AddSingleton<MVVM_22_WpfCap.Model.IWpfCapModel, MVVM_22_WpfCap.Model.CWpfCapModel>()
                .AddSingleton<MVVM_22_CTWpfCap.Model.IWpfCapModel, MVVM_22_CTWpfCap.Model.CWpfCapModel>()
                .AddTransient<MVVM_22_WpfCap.ViewModels.WpfCapViewModel>()
                .AddTransient<MVVM_22_CTWpfCap.ViewModels.WpfCapViewModel>()
                .AddSingleton<MVVM_28_1_CTDataGridExt.Services.IPersonService, MVVM_28_1_CTDataGridExt.Services.PersonService>()
                .AddSingleton<MVVM_25_RichTextEdit.Models.IRichTextEditModel, MVVM_25_RichTextEdit.Models.RichTextEditModel>()
                .AddSingleton<MVVM_36_ComToolKtSavesWork.Models.IUserRepository, MVVM_36_ComToolKtSavesWork.Models.UserRepository>()
                .AddSingleton<MVVM_36_ComToolKtSavesWork.Models.ICommunityToolkit2Model, MVVM_36_ComToolKtSavesWork.Models.CommunityToolkit2Model>()
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

                .AddSingleton<MVVM_39_MultiModelTest.Models.ISystemModel, MVVM_39_MultiModelTest.Models.SystemModel>()
                .AddScoped<MVVM_39_MultiModelTest.Models.IScopedModel, MVVM_39_MultiModelTest.Models.ScopedModel>()

                .AddSingleton<MVVM_40_Wizzard.Models.IWizzardModel, MVVM_40_Wizzard.Models.WizzardModel>()
                .AddSingleton<Sudoku_Base.Models.Interfaces.ISudokuModel, Sudoku_Base.Models.SudokuModel>()
                ;


            var sp = sc.BuildServiceProvider();
            IoC.Configure(sp);
        }
    }
}
namespace MVVM_AllExamples.Models { }
namespace MVVM_AllExamples.ValueConverter { }
namespace MVVM_AllExamples.Services { }
