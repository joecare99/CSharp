// ***********************************************************************
// Assembly         : MVVM_22_CTWpfCap
// Author           : Mir
// Created          : 08-18-2022
//
// Last Modified By : Mir
// Last Modified On : 08-16-2022
// ***********************************************************************
// <copyright file="RowData.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;

namespace MVVM_22_CTWpfCap.ViewModels.Interfaces;

public interface IRowData : INotifyPropertyChanged
{
    IRelayCommand<object>? MoveLeftCommand { get; }
    IRelayCommand<object>? MoveRightCommand { get; }
    int RowId { get; set; }
    int[] TileColor { get; }
    IRowData This { get; }
    int Length { get; }
    void OnPropertyChanged(string propertyName);  
}