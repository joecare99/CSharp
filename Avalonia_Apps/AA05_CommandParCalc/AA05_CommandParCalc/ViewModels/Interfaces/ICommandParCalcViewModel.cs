// ***********************************************************************
// Assembly         : AA05_CommandParCalc
// Author           : Mir
// Created          : 01-11-2025
//
// Last Modified By : Mir
// Last Modified On : 01-12-2025
// ***********************************************************************
// <copyright file="CommandParCalcViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.Input;
using System;

namespace AA05_CommandParCalc.ViewModels.Interfaces
{
    public interface ICommandParCalcViewModel
    {
        string Greeting { get; }
        string Title { get; }
        DateTime Now { get; }

        IRelayCommand HomeCommand { get; }
        IRelayCommand ConfigCommand { get; }
        IRelayCommand ProcessCommand { get; }
        IRelayCommand ActionsCommand { get; }
        IRelayCommand MacrosCommand { get; }
        IRelayCommand ReportsCommand { get; }
        IRelayCommand HistoryCommand { get; }
    }
}
