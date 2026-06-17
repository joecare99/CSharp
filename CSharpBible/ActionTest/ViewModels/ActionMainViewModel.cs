// ***********************************************************************
// Assembly         : ActionTest
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 02-02-2020
// ***********************************************************************
// <copyright file="FrmActionMain.Designer.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace ActionTest.ViewModels;

public partial class ActionMainViewModel : ObservableObject, IActionMainViewModel
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CloseClickCommand))]
    public partial string EnterQuitText { get; set; } = "";

    public Action<object>? ExitAction { get; set; }

    [RelayCommand(CanExecute = nameof(CanClose))]
    private void CloseClick()
    {
         ExitAction?.Invoke(this);
    }

    private bool CanClose()
    {
        // Implement the logic to determine if the close command can execute
        return EnterQuitText.ToLower() == "quit" || EnterQuitText.ToLower() == "exit";
    }
}