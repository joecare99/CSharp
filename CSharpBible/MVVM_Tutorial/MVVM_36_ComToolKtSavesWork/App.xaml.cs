// ***********************************************************************
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
using Microsoft.Extensions.DependencyInjection;
using MVVM_36_ComToolKtSavesWork.Models;
using MVVM_36_ComToolKtSavesWork.ViewModels;
using System.Windows;
using System.Windows.Navigation;

namespace MVVM_36_ComToolKtSavesWork
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            CommunityToolkit.Mvvm.DependencyInjection.Ioc.Default.ConfigureServices(
                new ServiceCollection()
                .AddSingleton<IUserRepository, UserRepository>()
                .AddSingleton<ICommunityToolkit2Model, CommunityToolkit2Model>()
                .AddTransient<MainWindowViewModel>()
                .AddTransient<CommunityToolkit2ViewModel>()
                .AddTransient<UserInfoViewModel>()
                .AddTransient<LoginViewModel>()
                .BuildServiceProvider()
                );
        }
    }
}

namespace MVVM_36_ComToolKtSavesWork.Models { }
namespace MVVM_36_ComToolKtSavesWork.ValueConverter { }
namespace MVVM_36_ComToolKtSavesWork.Services { }