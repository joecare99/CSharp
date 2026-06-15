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
using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;

namespace ActionTest.ViewModels;

public interface IActionMainViewModel : INotifyPropertyChanged
{
    IRelayCommand CloseClickCommand { get; } 
    string EnterQuitText { get; set; }
    Action<object>? ExitAction { get; set; }
}