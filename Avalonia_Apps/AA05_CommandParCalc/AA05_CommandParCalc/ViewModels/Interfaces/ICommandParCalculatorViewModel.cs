// ***********************************************************************
// Assembly         : AA05_CommandParCalc
// Author           : Mir
// Created          : 05-11-2023
//
// Last Modified By : Mir
// Last Modified On : 05-19-2023
// ***********************************************************************
// <copyright file="CommandParCalculatorViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using AA05_CommandParCalc.Data;
using CommunityToolkit.Mvvm.Input;

namespace AA05_CommandParCalc.ViewModels.Interfaces;

public interface ICommandParCalculatorViewModel
{
    double Accumulator { get; }
    double Memory { get; }
    double Register { get; }
    string Status { get; }

    public IRelayCommand<ECommands> CalculatorCommand { get; }
    public IRelayCommand<ENumbers> NumberCommand { get; }
    public IRelayCommand<EOperations> OperatorCommand { get; }
}