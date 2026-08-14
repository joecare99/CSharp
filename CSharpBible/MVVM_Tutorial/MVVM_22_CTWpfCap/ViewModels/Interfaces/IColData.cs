// ***********************************************************************
// Assembly         : MVVM_22_CTWpfCap
// Author           : Mir
// Created          : 08-18-2022
//
// Last Modified By : Mir
// Last Modified On : 08-16-2022
// ***********************************************************************
// <copyright file="ColData.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;

namespace MVVM_22_CTWpfCap.ViewModels.Interfaces;

public interface IColData: INotifyPropertyChanged
{
    int this[int ix] { get; }
    IRelayCommand<object>? MoveDownCommand { get; }
    IRelayCommand<object>? MoveUpCommand { get;  }
    int ColId { get; set; }
    IColData This { get; }
    int Length { get; }
}