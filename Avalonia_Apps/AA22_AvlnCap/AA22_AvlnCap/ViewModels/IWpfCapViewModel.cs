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
using System.Collections.ObjectModel;

namespace AA22_AvlnCap.ViewModels;

public interface IWpfCapViewModel
{
    ObservableCollection<RowData> Rows { get; set; }
    ObservableCollection<ColData> Cols { get; set; }

    IRelayCommand ShuffleCommand { get; }
    IRelayCommand<ColData> MoveUpCommand { get; }
    IRelayCommand<ColData> MoveDownCommand { get; }
    IRelayCommand<RowData> MoveLeftCommand { get; }
    IRelayCommand<RowData> MoveRightCommand { get; }
}