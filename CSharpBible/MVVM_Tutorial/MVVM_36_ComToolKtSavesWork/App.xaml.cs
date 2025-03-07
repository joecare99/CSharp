﻿// ***********************************************************************
// Assembly         : MVVM_36_ComToolKtSavesWork
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
using BaseLib.Helper;
using MVVM_36_ComToolKtSavesWork.Models;
using MVVM_36_ComToolKtSavesWork.ViewModels;
using System.Windows;

namespace MVVM_36_ComToolKtSavesWork
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
                .AddSingleton<ICommunityToolkit2Model, CommunityToolkit2Model>()
                .AddSingleton<IMessenger>(WeakReferenceMessenger.Default)
                .AddTransient<MainWindowViewModel>()
                .AddTransient<CommunityToolkit2ViewModel>()
                .AddTransient<UserInfoViewModel>()
                .AddTransient<LoginViewModel>()
                .BuildServiceProvider();
            IoC.GetReqSrv = srvProv.GetRequiredService;
            IoC.GetSrv= srvProv.GetService;
        }
    }
}

namespace MVVM_36_ComToolKtSavesWork.Models { }
namespace MVVM_36_ComToolKtSavesWork.ValueConverter { }
namespace MVVM_36_ComToolKtSavesWork.Services { }