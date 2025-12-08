// ***********************************************************************
// Assembly         : Avalonia_App02
// Author           : Mir
// Created          : 01-11-2025
//
// Last Modified By : Mir
// Last Modified On : 01-12-2025
// ***********************************************************************
// <copyright file="SomeTemplateViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.Input;
using System;

namespace Avalonia_App02.ViewModels.Interfaces
{
    public interface ISomeTemplateViewModel
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