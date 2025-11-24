// ***********************************************************************
// Assembly         : MVVM_25_RichTextEdit
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-24-2022
// ***********************************************************************
// <copyright file="MainWindowViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;

namespace AA25_RichTextEdit.ViewModels.Interfaces;

public interface IRichTextEditViewModel: INotifyPropertyChanged
{

    IRelayCommand NewTextCommand { get; }
    IRelayCommand OpenTextCommand { get; }
    IRelayCommand SaveTextCommand { get; }
    IRelayCommand PrintTextCommand { get; }
    IRelayCommand ExitCommand { get; }
    Action CloseApp { get; set; }
    string AllImgSource { get; }
    DateTime Now { get; }
}