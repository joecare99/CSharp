// ***********************************************************************
// Assembly         : MVVM_33a_CTEvents_To_Commands
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-24-2022
// ***********************************************************************
// <copyright file="EventsBindingViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MVVM_33a_CTEvents_To_Commands.ViewModels
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="BaseViewModelCT" />
    /// </summary>
    /// <seealso cref="BaseViewModelCT" />
    public partial class EventsBindingViewModel : BaseViewModelCT
    {
        #region Properties
        [ObservableProperty]
        private string _state = "";
        #endregion
        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public EventsBindingViewModel()
        { }

        [RelayCommand]
        private void LostFocus(object? _)
            => State = "Lost focus";

        [RelayCommand]
        private void GotFocus(object? _)
            => State = "Got focus";

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
