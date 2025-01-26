// ***********************************************************************
// Assembly         : Calc32WPF_net
// Author           : Mir
// Created          : 12-22-2021
//
// Last Modified By : Mir
// Last Modified On : 10-22-2022
// ***********************************************************************
// <copyright file="CalculatorViewModel.cs" company="Calc32WPF_net">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;

namespace Calc32.ViewModels.Interfaces;

public interface ICalculatorViewModel : INotifyPropertyChanged
{
    int Accumulator { get; set; }
    int Memory { get;  }
    string OperationText { get; }
    IRelayCommand BackSpaceCommand { get; }
    IRelayCommand<object?> NumberCommand { get; }
    IRelayCommand<object?> OperationCommand { get; }
}