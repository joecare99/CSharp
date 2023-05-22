﻿// ***********************************************************************
// Assembly         : WpfApp
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MVVM_33_Events_To_Commands.ViewModels
{
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
        #endregion 
        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public EventsBindingViewModel()
        {
            LostFocusCommand = new((s) => State = "Lost focus");
            GotFocusCommand = new((s) => State = "Got focus");
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
}
