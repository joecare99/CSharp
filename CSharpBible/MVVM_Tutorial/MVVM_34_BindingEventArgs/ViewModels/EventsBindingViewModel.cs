﻿// ***********************************************************************
// Assembly         : MVVM_34_BindingEventArgs
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
using MVVM.ViewModel;
using System.Windows.Input;

namespace MVVM_34_BindingEventArgs.ViewModels;

/// <summary>
/// Class MainWindowViewModel.
/// Implements the <see cref="BaseViewModel" />
/// </summary>
/// <seealso cref="BaseViewModel" />
public class EventsBindingViewModel : BaseViewModel
{
    #region Properties
    private string _state="";
    public string State { get => _state; set => SetProperty(ref _state, value); }
 
    public DelegateCommand LostFocusCommand { get; set; }
    public DelegateCommand GotFocusCommand { get; set; }
    public DelegateCommand KeyDownCommand { get; set; }
    #endregion 
    #region Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public EventsBindingViewModel()
    {
        LostFocusCommand = new((s) => State = "Lost focus");
        GotFocusCommand = new((s) => State = "Got focus");
        KeyDownCommand = new((s) => State = $"TextChanged({(s as KeyEventArgs)?.Key})",
            (s) => s is KeyEventArgs { Key: Key.Enter or Key.Escape });
    }

#if !NET5_0_OR_GREATER
    /// <summary>
    /// Finalizes an instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    ~EventsBindingViewModel()
    {
        return;
    }
#endif
    #endregion
}
