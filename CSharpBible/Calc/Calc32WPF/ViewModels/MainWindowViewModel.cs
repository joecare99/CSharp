// ***********************************************************************
// Assembly         : Calc32WPF_net
// Author           : Mir
// Created          : 12-22-2021
//
// Last Modified By : Mir
// Last Modified On : 10-22-2022
// ***********************************************************************
// <copyright file="MainWindowViewModel.cs" company="Calc32WPF_net">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Calc32.ViewModels;
using Calc32.Models.Interfaces;
using CommunityToolkit.Mvvm.DependencyInjection;

/// <summary>
/// The ViewModel namespace.
/// </summary>
namespace Calc32WPF.ViewModels
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="T:MVVM.ViewModel.BaseViewModel" />
    /// </summary>
    public class MainWindowViewModel : CalculatorViewModel
    {
        public MainWindowViewModel() : base(Ioc.Default.GetRequiredService<ICalculatorClass>())
        {
        }
    }
}
