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

namespace AA22_AvlnCap.ViewModels.Interfaces;

public interface IColData
{
    int this[int ix] { get; }

    int ColId { get; set; }
    object This { get; }
    IRelayCommand<object>? MoveUpCommand { get; }
    IRelayCommand<object>? MoveDownCommand { get; }
}