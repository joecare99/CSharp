// ***********************************************************************
// Assembly         : Calc64WF
// Author           : Mir
// Created          : 08-28-2022
//
// Last Modified By : Mir
// Last Modified On : 08-31-2022
// ***********************************************************************
// <copyright file="FrmCalc64MainViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Calc64WF.ViewModels.Interfaces;

public interface IFrmCalc64MainViewModel: INotifyPropertyChanged
{
    event EventHandler<(string prop, object? oldVal, object? newVal)> OnDataChanged;

    long Accumulator { get; }
    long Memory { get; }
    long Register { get; }
    string OperationText { get; }

    IRelayCommand<object?> NumberCommand { get; }
    IRelayCommand<object?> OperationCommand { get; }
    IRelayCommand<object?> BackSpaceCommand { get; }

    event EventHandler CloseForm;

    void btnClose_Click(object sender, object tag, EventArgs e);
    void frm_KeyDown(object sender, object tag, KeyEventArgs e);
}