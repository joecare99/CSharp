// ***********************************************************************
// Assembly         : AA22_AvlnCap
// Author           : Mir
// Created          : 08-18-2022
//
// Last Modified By : Mir
// Last Modified On : 08-16-2022
// ***********************************************************************
// <copyright file="WpfCapViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;

namespace AA22_AvlnCap.ViewModels.Interfaces;

public interface IRowData: INotifyPropertyChanged
{
    int RowId { get; }
    IRelayCommand<object>? MoveLeftCommand { get; }
    IRelayCommand<object>? MoveRightCommand { get; }
    int[] TileColor { get; }
    object This { get; }

    void OnPropertyChanged(string v);
}