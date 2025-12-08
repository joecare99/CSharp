// ***********************************************************************
// Assembly         : AA22_AvlnCap2
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
using AA22_AvlnCap2.Model;
using System.Collections.ObjectModel;

namespace AA22_AvlnCap2.ViewModels.Interfaces;

public interface IWpfCapViewModel
{
    ObservableCollection<IRowData> Rows { get; set; }
    ObservableCollection<IColData> Cols { get; set; }
    IWpfCapModel Model { get; }

    IRelayCommand ShuffleCommand { get; }
    IRelayCommand<object> MoveLeftCommand { get; }
    IRelayCommand<object> MoveRightCommand { get; }
    IRelayCommand<object> MoveUpCommand { get; }
    IRelayCommand<object> MoveDownCommand { get; }
}